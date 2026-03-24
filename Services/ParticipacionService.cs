using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;

namespace JuegosTorneosApi.Services;

public class ParticipacionService : IParticipacionService
{
    private readonly FirestoreDb _firestoreDb;

    public ParticipacionService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<ApiResponse<ParticipacionResponse>> InscribirseAsync(string torneoId, string jugadorId, InscribirseTorneoRequest request)
    {
        // 1. Validar que el torneo exista
        DocumentReference torneoRef = _firestoreDb.Collection("torneos").Document(torneoId);
        DocumentSnapshot torneoSnapshot = await torneoRef.GetSnapshotAsync();
        
        if (!torneoSnapshot.Exists)
        {
            return ApiResponse<ParticipacionResponse>.Fail("El torneo especificado no existe.");
        }

        Torneo torneo = torneoSnapshot.ConvertTo<Torneo>();

        // 2. Validar estado "próximo"
        if (torneo.Estado.ToLower() != "próximo")
        {
            return ApiResponse<ParticipacionResponse>.Fail($"El torneo no está disponible para inscripciones. Estado actual: {torneo.Estado}.");
        }

        // 3. Validar límite de inscripción
        if (DateTime.UtcNow > torneo.FechaLimiteInscripcion)
        {
            return ApiResponse<ParticipacionResponse>.Fail("La fecha límite de inscripción ya ha pasado.");
        }

        // 4. Validar cupos disponibles
        if (torneo.ParticipantesActuales >= torneo.MaxParticipantes)
        {
            return ApiResponse<ParticipacionResponse>.Fail("El torneo ha alcanzado el límite máximo de participantes.");
        }

        // 5. Validar que el jugador no esté ya inscrito
        Query queryInscripcion = _firestoreDb.Collection("participaciones")
            .WhereEqualTo("torneoId", torneoId)
            .WhereEqualTo("jugadorId", jugadorId);
        
        QuerySnapshot inscripcionSnapshot = await queryInscripcion.GetSnapshotAsync();
        if (inscripcionSnapshot.Documents.Count > 0)
        {
            return ApiResponse<ParticipacionResponse>.Fail("El jugador ya se encuentra inscrito en este torneo.");
        }

        // 6. Validar nivel del jugador
        DocumentReference jugadorRef = _firestoreDb.Collection("jugadores").Document(jugadorId);
        DocumentSnapshot jugadorSnapshot = await jugadorRef.GetSnapshotAsync();
        
        if (!jugadorSnapshot.Exists) return ApiResponse<ParticipacionResponse>.Fail("El jugador autenticado no existe en la BD.");

        // Intentar obtener "nivel" o usar "puntosGlobales" como fallback si la estructura de Jugadores no lo tiene explícito
        int nivelJugador = 0;
        if (jugadorSnapshot.TryGetValue("nivel", out int nivelReal)) {
            nivelJugador = nivelReal;
        } else if (jugadorSnapshot.TryGetValue("puntosGlobales", out int puntos)) {
            nivelJugador = puntos;
        }

        if (torneo.MinNivel > 0 && nivelJugador < torneo.MinNivel)
        {
            return ApiResponse<ParticipacionResponse>.Fail($"El nivel del jugador ({nivelJugador}) es menor al mínimo requerido ({torneo.MinNivel}).");
        }
        if (torneo.MaxNivel > 0 && nivelJugador > torneo.MaxNivel)
        {
            return ApiResponse<ParticipacionResponse>.Fail($"El nivel del jugador ({nivelJugador}) supera el máximo permitido ({torneo.MaxNivel}).");
        }

        // 7. Validar pago
        if (torneo.PrecioInscripcion > 0 && !request.Pagado)
        {
            return ApiResponse<ParticipacionResponse>.Fail("El torneo requiere una couta de inscripción. El pago no ha sido procesado (pagado=false).");
        }

        // 8. Crear Participacion y actualizar Torneo
        Participacion nuevaParticipacion = new Participacion
        {
            JugadorId = jugadorId,
            TorneoId = torneoId,
            EquipoId = request.EquipoId,
            Estado = "activo",
            Posicion = 0,
            PuntosObtenidos = 0,
            PartidasJugadas = 0,
            Victorias = 0,
            Derrotas = 0,
            FechaInscripcion = DateTime.UtcNow,
            FechaEliminacion = null,
            Estadisticas = new EstadisticasParticipacion { Asesinatos = 0, Asistencias = 0, DañoCausado = 0, Muertes = 0 },
            Penalizaciones = 0,
            Pagado = request.Pagado
        };

        // Guardar Participacion
        DocumentReference partRef = await _firestoreDb.Collection("participaciones").AddAsync(nuevaParticipacion);
        nuevaParticipacion.Id = partRef.Id;

        // Incrementar participantesActuales
        await torneoRef.UpdateAsync("participantesActuales", FieldValue.Increment(1));

        return ApiResponse<ParticipacionResponse>.Success(MapearAResponse(nuevaParticipacion), "Inscripción realizada con éxito.");
    }

    public async Task<ApiResponse<List<MisTorneosResponse>>> ObtenerMisTorneosAsync(string jugadorId)
    {
        Query query = _firestoreDb.Collection("participaciones").WhereEqualTo("jugadorId", jugadorId);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();
        
        List<MisTorneosResponse> lista = new List<MisTorneosResponse>();

        foreach (var doc in snapshot.Documents)
        {
            if (!doc.Exists) continue;
            Participacion p = doc.ConvertTo<Participacion>();
            p.Id = doc.Id;

            // Fetch Torneo info
            DocumentSnapshot torneoSnapshot = await _firestoreDb.Collection("torneos").Document(p.TorneoId).GetSnapshotAsync();
            if (torneoSnapshot.Exists)
            {
                Torneo t = torneoSnapshot.ConvertTo<Torneo>();
                lista.Add(new MisTorneosResponse
                {
                    ParticipacionId = p.Id,
                    EstadoParticipacion = p.Estado,
                    FechaInscripcion = p.FechaInscripcion,
                    TorneoId = t.Id ?? torneoSnapshot.Id,
                    NombreTorneo = t.Nombre,
                    JuegoTorneo = t.Juego,
                    FechaInicioTorneo = t.FechaInicio,
                    EstadoTorneo = t.Estado
                });
            }
        }

        return ApiResponse<List<MisTorneosResponse>>.Success(lista);
    }

    public async Task<ApiResponse<ParticipacionResponse>> AbandonarTorneoAsync(string participacionId, string jugadorId)
    {
        DocumentReference partRef = _firestoreDb.Collection("participaciones").Document(participacionId);
        DocumentSnapshot partSnapshot = await partRef.GetSnapshotAsync();

        if (!partSnapshot.Exists) return ApiResponse<ParticipacionResponse>.Fail("La participación especificada no existe.");

        Participacion participacion = partSnapshot.ConvertTo<Participacion>();
        participacion.Id = partSnapshot.Id;

        // Regla: Solo el jugador dueño puede abandonar
        if (participacion.JugadorId != jugadorId)
        {
            return ApiResponse<ParticipacionResponse>.Fail("No tienes permiso para abandonar esta participación.");
        }

        if (participacion.Estado == "abandonado")
        {
            return ApiResponse<ParticipacionResponse>.Fail("Ya has abandonado este torneo previamente.");
        }

        // Fetch Torneo state
        DocumentReference torneoRef = _firestoreDb.Collection("torneos").Document(participacion.TorneoId);
        DocumentSnapshot torneoSnapshot = await torneoRef.GetSnapshotAsync();

        if (!torneoSnapshot.Exists) return ApiResponse<ParticipacionResponse>.Fail("El torneo relacional no existe.");

        Torneo torneo = torneoSnapshot.ConvertTo<Torneo>();

        // Regla: Solo permitir abandono si el estado del torneo es \"próximo\"
        if (torneo.Estado.ToLower() != "próximo")
        {
            return ApiResponse<ParticipacionResponse>.Fail($"No se puede abandonar un torneo en estado '{torneo.Estado}'. Debe estar en estado 'próximo'.");
        }

        // Logica para cambiar estado a "abandonado"
        Dictionary<string, object> partUpdates = new Dictionary<string, object>
        {
            { "estado", "abandonado" },
            { "fechaEliminacion", DateTime.UtcNow }
        };

        await partRef.UpdateAsync(partUpdates);

        // Regla: Decrementar participantesActuales de manera segura (evitar decrementos incorrectos)
        if (torneo.ParticipantesActuales > 0)
        {
            await torneoRef.UpdateAsync("participantesActuales", FieldValue.Increment(-1));
        }

        DocumentSnapshot updatedPartSnapshot = await partRef.GetSnapshotAsync();
        Participacion updatedPart = updatedPartSnapshot.ConvertTo<Participacion>();
        updatedPart.Id = updatedPartSnapshot.Id;

        return ApiResponse<ParticipacionResponse>.Success(MapearAResponse(updatedPart), "Has abandonado el torneo con éxito.");
    }

    private ParticipacionResponse MapearAResponse(Participacion p)
    {
        return new ParticipacionResponse
        {
            Id = p.Id,
            JugadorId = p.JugadorId,
            TorneoId = p.TorneoId,
            EquipoId = p.EquipoId,
            Estado = p.Estado,
            Posicion = p.Posicion,
            PuntosObtenidos = p.PuntosObtenidos,
            PartidasJugadas = p.PartidasJugadas,
            Victorias = p.Victorias,
            Derrotas = p.Derrotas,
            FechaInscripcion = p.FechaInscripcion,
            FechaEliminacion = p.FechaEliminacion,
            Estadisticas = p.Estadisticas,
            Penalizaciones = p.Penalizaciones,
            Pagado = p.Pagado
        };
    }
}
