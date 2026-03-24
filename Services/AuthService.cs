using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using BCrypt.Net;

namespace JuegosTorneosApi.Services;

public class AuthService : IAuthService
{
    private readonly FirestoreDb _firestoreDb;
    private const string ColeccionJugadores = "jugadores";

    public AuthService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<ApiResponse<JugadorPublicoResponse>> RegistroAsync(RegistroRequest request)
    {
        CollectionReference jugadoresRef = _firestoreDb.Collection(ColeccionJugadores);

        // Validar correo único
        Query queryCorreo = jugadoresRef.WhereEqualTo("correo", request.Correo);
        QuerySnapshot snapshotCorreo = await queryCorreo.GetSnapshotAsync();
        if (snapshotCorreo.Documents.Count > 0)
        {
            return ApiResponse<JugadorPublicoResponse>.Fail("El correo ya está registrado.");
        }

        // Validar nombreUsuario único
        Query queryUsuario = jugadoresRef.WhereEqualTo("nombreUsuario", request.NombreUsuario);
        QuerySnapshot snapshotUsuario = await queryUsuario.GetSnapshotAsync();
        if (snapshotUsuario.Documents.Count > 0)
        {
            return ApiResponse<JugadorPublicoResponse>.Fail("El nombre de usuario ya está en uso.");
        }

        // Crear jugador
        Jugador nuevoJugador = new Jugador
        {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Correo = request.Correo,
            Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena), // Hash con BCrypt
            NombreUsuario = request.NombreUsuario,
            Edad = request.Edad,
            Pais = request.Pais,
            Rol = "jugador", // Rol por defecto
            Activo = true,
            PuntosGlobales = 0,
            TorneosGanados = 0,
            FechaRegistro = DateTime.UtcNow,
            Conectado = false,
            UltimaConexion = null
        };

        // Guardar en Firestore con un nuevo ID automático
        DocumentReference docRef = await jugadoresRef.AddAsync(nuevoJugador);
        nuevoJugador.Id = docRef.Id;

        // Armar respuesta sin contraseña
        var response = new JugadorPublicoResponse
        {
            Id = nuevoJugador.Id,
            Nombre = nuevoJugador.Nombre,
            Apellido = nuevoJugador.Apellido,
            Correo = nuevoJugador.Correo,
            NombreUsuario = nuevoJugador.NombreUsuario,
            Edad = nuevoJugador.Edad,
            Pais = nuevoJugador.Pais,
            Rol = nuevoJugador.Rol,
            Activo = nuevoJugador.Activo,
            PuntosGlobales = nuevoJugador.PuntosGlobales,
            TorneosGanados = nuevoJugador.TorneosGanados,
            FechaRegistro = nuevoJugador.FechaRegistro,
            Conectado = nuevoJugador.Conectado,
            UltimaConexion = nuevoJugador.UltimaConexion
        };

        return ApiResponse<JugadorPublicoResponse>.Success(response, "Jugador registrado exitosamente.");
    }
}
