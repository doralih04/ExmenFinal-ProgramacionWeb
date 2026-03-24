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
        // 1. Validar que el juego exista
        DocumentReference juegoRef = _firestoreDb.Collection("juegos").Document(request.Juego);
        DocumentSnapshot juegoSnapshot = await juegoRef.GetSnapshotAsync();
        if (!juegoSnapshot.Exists)
        {
            return ApiResponse<TorneoResponse>.Fail("El juego especificado no existe en la base de datos.");
        }

        // 2. Validar que el organizador exista y sea "organizador" o "admin"
        DocumentReference organizadorRef = _firestoreDb.Collection("jugadores").Document(request.Organizador);
        DocumentSnapshot organizadorSnapshot = await organizadorRef.GetSnapshotAsync();
        if (!organizadorSnapshot.Exists)
        {
            return ApiResponse<TorneoResponse>.Fail("El organizador especificado no existe en la base de datos.");
        }

        var rolOrganizador = organizadorSnapshot.GetValue<string>("rol");
        if (rolOrganizador != "organizador" && rolOrganizador != "admin")
        {
            return ApiResponse<TorneoResponse>.Fail("El usuario especificado no tiene permiso para organizar torneos (requiere rol organizador o admin).");
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
            ParticipantesActuales = 0, // Regla de inicialización
            PrecioInscripcion = request.PrecioInscripcion,
            PremioTotal = request.PremioTotal,
            FechaInicio = request.FechaInicio.ToUniversalTime(),
            FechaFin = request.FechaFin.ToUniversalTime(),
            FechaLimiteInscripcion = request.FechaLimiteInscripcion.ToUniversalTime(),
            MinNivel = request.MinNivel,
            MaxNivel = request.MaxNivel,
            RequiereEquipo = request.RequiereEquipo,
            TamanioEquipo = request.TamanioEquipo,
            FechaCreacion = DateTime.UtcNow, // Regla de inicialización
            ReglasModificadas = request.ReglasModificadas
        };

        DocumentReference docRef = await _firestoreDb.Collection(ColeccionTorneos).AddAsync(nuevoTorneo);
        nuevoTorneo.Id = docRef.Id;

        return ApiResponse<TorneoResponse>.Success(MapearAResponse(nuevoTorneo), "Torneo creado exitosamente.");
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
