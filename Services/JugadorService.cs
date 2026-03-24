using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;

namespace JuegosTorneosApi.Services;

public class JugadorService : IJugadorService
{
    private readonly FirestoreDb _firestoreDb;
    private const string ColeccionJugadores = "jugadores";

    public JugadorService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<ApiResponse<JugadorPublicoResponse>> ObtenerJugadorAsync(string id)
    {
        DocumentReference docRef = _firestoreDb.Collection(ColeccionJugadores).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return ApiResponse<JugadorPublicoResponse>.Fail("Jugador no encontrado.");
        }

        Jugador jugador = snapshot.ConvertTo<Jugador>();
        jugador.Id = snapshot.Id;

        return ApiResponse<JugadorPublicoResponse>.Success(MapearAVersionPublica(jugador));
    }

    public async Task<ApiResponse<JugadorPublicoResponse>> ActualizarPerfilAsync(string id, ActualizarPerfilJugadorRequest request, string currentUserId, string currentUserRole)
    {
        // Validación de autorización: solo el propietario o un admin pueden modificar el perfil.
        if (id != currentUserId && currentUserRole != "admin")
        {
            return ApiResponse<JugadorPublicoResponse>.Fail("No tienes permiso para modificar a este jugador.");
        }

        DocumentReference docRef = _firestoreDb.Collection(ColeccionJugadores).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return ApiResponse<JugadorPublicoResponse>.Fail("Jugador no encontrado.");
        }

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "nombre", request.Nombre },
            { "apellido", request.Apellido },
            { "edad", request.Edad },
            { "pais", request.Pais }
        };

        await docRef.UpdateAsync(updates);

        // Retornar en tiempo real la versión actualizada mapeada
        DocumentSnapshot updatedSnapshot = await docRef.GetSnapshotAsync();
        Jugador jugadorUpdated = updatedSnapshot.ConvertTo<Jugador>();
        jugadorUpdated.Id = updatedSnapshot.Id;

        return ApiResponse<JugadorPublicoResponse>.Success(MapearAVersionPublica(jugadorUpdated), "Perfil actualizado correctamente.");
    }

    private JugadorPublicoResponse MapearAVersionPublica(Jugador jugador)
    {
        return new JugadorPublicoResponse
        {
            Id = jugador.Id,
            Nombre = jugador.Nombre,
            Apellido = jugador.Apellido,
            Correo = jugador.Correo,
            NombreUsuario = jugador.NombreUsuario,
            Edad = jugador.Edad,
            Pais = jugador.Pais,
            Rol = jugador.Rol,
            Activo = jugador.Activo,
            PuntosGlobales = jugador.PuntosGlobales,
            TorneosGanados = jugador.TorneosGanados,
            FechaRegistro = jugador.FechaRegistro,
            Conectado = jugador.Conectado,
            UltimaConexion = jugador.UltimaConexion
        };
    }
}
