using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;

namespace JuegosTorneosApi.Services;

public class TorneoService : ITorneoService
{
    private readonly FirestoreDb _firestoreDb;
    private const string ColeccionTorneos = "torneos";

    public TorneoService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<ApiResponse<TorneoResponse>> CrearTorneoAsync(CrearTorneoRequest request)
    {
        // 1. Validar juego
        DocumentReference juegoRef = _firestoreDb.Collection("juegos").Document(request.Juego);
        DocumentSnapshot juegoSnapshot = await juegoRef.GetSnapshotAsync();
        if (!juegoSnapshot.Exists) return ApiResponse<TorneoResponse>.Fail("El juego especificado no existe.");

        // 2. Validar organizador
        DocumentReference organizadorRef = _firestoreDb.Collection("jugadores").Document(request.Organizador);
        DocumentSnapshot organizadorSnapshot = await organizadorRef.GetSnapshotAsync();
        if (!organizadorSnapshot.Exists) return ApiResponse<TorneoResponse>.Fail("El organizador especificado no existe.");

        var rolOrganizador = organizadorSnapshot.GetValue<string>("rol");
        if (rolOrganizador != "organizador" && rolOrganizador != "admin")
        {
            return ApiResponse<TorneoResponse>.Fail("El usuario no tiene rol de organizador o admin.");
        }

        Torneo nuevoTorneo = new Torneo
        {
            Nombre = request.Nombre,
            Juego = request.Juego,
            Organizador = request.Organizador,
            Descripcion = request.Descripcion,
            Estado = request.Estado,
            Formato = request.Formato,
            MaxParticipantes = request.MaxParticipantes,
            ParticipantesActuales = 0, // Regla
            PrecioInscripcion = request.PrecioInscripcion,
            PremioTotal = request.PremioTotal,
            FechaInicio = request.FechaInicio.ToUniversalTime(),
            FechaFin = request.FechaFin.ToUniversalTime(),
            FechaLimiteInscripcion = request.FechaLimiteInscripcion.ToUniversalTime(),
            MinNivel = request.MinNivel,
            MaxNivel = request.MaxNivel,
            RequiereEquipo = request.RequiereEquipo,
            TamanioEquipo = request.TamanioEquipo,
            FechaCreacion = DateTime.UtcNow, // Regla
            ReglasModificadas = request.ReglasModificadas
        };

        DocumentReference docRef = await _firestoreDb.Collection(ColeccionTorneos).AddAsync(nuevoTorneo);
        nuevoTorneo.Id = docRef.Id;

        return ApiResponse<TorneoResponse>.Success(MapearAResponse(nuevoTorneo), "Torneo creado exitosamente.");
    }

    public async Task<ApiResponse<List<TorneoResponse>>> ObtenerTorneosAsync(string? juego, string? estado, string? formato)
    {
        Query query = _firestoreDb.Collection(ColeccionTorneos);

        if (!string.IsNullOrEmpty(juego))
            query = query.WhereEqualTo("juego", juego);
        
        if (!string.IsNullOrEmpty(estado))
            query = query.WhereEqualTo("estado", estado);

        if (!string.IsNullOrEmpty(formato))
            query = query.WhereEqualTo("formato", formato);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();
        List<TorneoResponse> torneos = new();

        foreach (var doc in snapshot.Documents)
        {
            if (doc.Exists)
            {
                Torneo t = doc.ConvertTo<Torneo>();
                t.Id = doc.Id;
                torneos.Add(MapearAResponse(t));
            }
        }

        return ApiResponse<List<TorneoResponse>>.Success(torneos);
    }

    public async Task<ApiResponse<TorneoResponse>> ObtenerTorneoAsync(string id)
    {
        DocumentReference docRef = _firestoreDb.Collection(ColeccionTorneos).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return ApiResponse<TorneoResponse>.Fail("Torneo no encontrado.");
        }

        Torneo torneo = snapshot.ConvertTo<Torneo>();
        torneo.Id = snapshot.Id;

        return ApiResponse<TorneoResponse>.Success(MapearAResponse(torneo));
    }

    public async Task<ApiResponse<TorneoResponse>> ActualizarTorneoAsync(string id, ActualizarTorneoRequest request)
    {
        DocumentReference docRef = _firestoreDb.Collection(ColeccionTorneos).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists) return ApiResponse<TorneoResponse>.Fail("Torneo no encontrado.");

        Torneo torneoExistente = snapshot.ConvertTo<Torneo>();

        // Validar que el Juego referenciado exista si es que fue modificado o re-inyectado
        DocumentReference juegoRef = _firestoreDb.Collection("juegos").Document(request.Juego);
        DocumentSnapshot juegoSnapshot = await juegoRef.GetSnapshotAsync();
        if (!juegoSnapshot.Exists) return ApiResponse<TorneoResponse>.Fail("El juego referenciado no existe.");

        // Regla: no permitir reducir maxParticipantes por debajo de participantesActuales
        if (request.MaxParticipantes < torneoExistente.ParticipantesActuales)
        {
            return ApiResponse<TorneoResponse>.Fail($"No se puede reducir el máximo de participantes por debajo de los participantes actuales ({torneoExistente.ParticipantesActuales}).");
        }

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "nombre", request.Nombre },
            { "juego", request.Juego },
            { "descripcion", request.Descripcion },
            { "formato", request.Formato },
            { "maxParticipantes", request.MaxParticipantes },
            { "precioInscripcion", request.PrecioInscripcion },
            { "premioTotal", request.PremioTotal },
            { "fechaInicio", request.FechaInicio.ToUniversalTime() },
            { "fechaFin", request.FechaFin.ToUniversalTime() },
            { "fechaLimiteInscripcion", request.FechaLimiteInscripcion.ToUniversalTime() },
            { "minNivel", request.MinNivel },
            { "maxNivel", request.MaxNivel },
            { "requiereEquipo", request.RequiereEquipo },
            { "tamanioEquipo", request.TamanioEquipo },
            { "reglasModificadas", request.ReglasModificadas }
        };

        await docRef.UpdateAsync(updates);

        DocumentSnapshot updatedSnapshot = await docRef.GetSnapshotAsync();
        Torneo torneoUpdated = updatedSnapshot.ConvertTo<Torneo>();
        torneoUpdated.Id = updatedSnapshot.Id;

        return ApiResponse<TorneoResponse>.Success(MapearAResponse(torneoUpdated), "Torneo actualizado exitosamente.");
    }

    public async Task<ApiResponse<TorneoResponse>> CambiarEstadoTorneoAsync(string id, CambiarEstadoTorneoRequest request)
    {
        DocumentReference docRef = _firestoreDb.Collection(ColeccionTorneos).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists) return ApiResponse<TorneoResponse>.Fail("Torneo no encontrado.");

        await docRef.UpdateAsync("estado", request.Estado);

        DocumentSnapshot updatedSnapshot = await docRef.GetSnapshotAsync();
        Torneo torneoUpdated = updatedSnapshot.ConvertTo<Torneo>();
        torneoUpdated.Id = updatedSnapshot.Id;

        return ApiResponse<TorneoResponse>.Success(MapearAResponse(torneoUpdated), $"Estado del torneo modificado a '{request.Estado}'.");
    }

    private TorneoResponse MapearAResponse(Torneo torneo)
    {
        return new TorneoResponse
        {
            Id = torneo.Id,
            Nombre = torneo.Nombre,
            Juego = torneo.Juego,
            Organizador = torneo.Organizador,
            Descripcion = torneo.Descripcion,
            Estado = torneo.Estado,
            Formato = torneo.Formato,
            MaxParticipantes = torneo.MaxParticipantes,
            ParticipantesActuales = torneo.ParticipantesActuales,
            PrecioInscripcion = torneo.PrecioInscripcion,
            PremioTotal = torneo.PremioTotal,
            FechaInicio = torneo.FechaInicio,
            FechaFin = torneo.FechaFin,
            FechaLimiteInscripcion = torneo.FechaLimiteInscripcion,
            MinNivel = torneo.MinNivel,
            MaxNivel = torneo.MaxNivel,
            RequiereEquipo = torneo.RequiereEquipo,
            TamanioEquipo = torneo.TamanioEquipo,
            FechaCreacion = torneo.FechaCreacion,
            ReglasModificadas = torneo.ReglasModificadas
        };
    }
}
