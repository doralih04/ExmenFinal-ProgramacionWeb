using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using JuegosTorneosApi.Config;
using BCrypt.Net;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace JuegosTorneosApi.Services;

public class AuthService : IAuthService
{
    private readonly FirestoreDb _firestoreDb;
    private readonly JwtSettings _jwtSettings;
    private const string ColeccionJugadores = "jugadores";

    public AuthService(FirestoreDb firestoreDb, IOptions<JwtSettings> jwtSettings)
    {
        _firestoreDb = firestoreDb;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<ApiResponse<JugadorPublicoResponse>> RegistroAsync(RegistroRequest request)
    {
        CollectionReference jugadoresRef = _firestoreDb.Collection(ColeccionJugadores);

        Query queryCorreo = jugadoresRef.WhereEqualTo("correo", request.Correo);
        QuerySnapshot snapshotCorreo = await queryCorreo.GetSnapshotAsync();
        if (snapshotCorreo.Documents.Count > 0)
        {
            return ApiResponse<JugadorPublicoResponse>.Fail("El correo ya está registrado.");
        }

        Query queryUsuario = jugadoresRef.WhereEqualTo("nombreUsuario", request.NombreUsuario);
        QuerySnapshot snapshotUsuario = await queryUsuario.GetSnapshotAsync();
        if (snapshotUsuario.Documents.Count > 0)
        {
            return ApiResponse<JugadorPublicoResponse>.Fail("El nombre de usuario ya está en uso.");
        }

        Jugador nuevoJugador = new Jugador
        {
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            Correo = request.Correo,
            Contrasena = BCrypt.Net.BCrypt.HashPassword(request.Contrasena),
            NombreUsuario = request.NombreUsuario,
            Edad = request.Edad,
            Pais = request.Pais,
            Rol = "jugador",
            Activo = true,
            PuntosGlobales = 0,
            TorneosGanados = 0,
            FechaRegistro = DateTime.UtcNow,
            Conectado = false,
            UltimaConexion = null
        };

        DocumentReference docRef = await jugadoresRef.AddAsync(nuevoJugador);
        nuevoJugador.Id = docRef.Id;

        return ApiResponse<JugadorPublicoResponse>.Success(MapearAVersionPublica(nuevoJugador), "Jugador registrado exitosamente.");
    }

    public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
    {
        CollectionReference jugadoresRef = _firestoreDb.Collection(ColeccionJugadores);
        Query queryCorreo = jugadoresRef.WhereEqualTo("correo", request.Correo).Limit(1);
        QuerySnapshot snapshotCorreo = await queryCorreo.GetSnapshotAsync();

        if (snapshotCorreo.Documents.Count == 0)
        {
            return ApiResponse<AuthResponse>.Fail("Credenciales inválidas.");
        }

        DocumentSnapshot document = snapshotCorreo.Documents[0];
        Jugador jugador = document.ConvertTo<Jugador>();
        jugador.Id = document.Id;

        if (!BCrypt.Net.BCrypt.Verify(request.Contrasena, jugador.Contrasena))
        {
            return ApiResponse<AuthResponse>.Fail("Credenciales inválidas.");
        }

        if (!jugador.Activo)
        {
            return ApiResponse<AuthResponse>.Fail("La cuenta se encuentra inactiva.");
        }

        // Actualizar conectado y ultimaConexion
        jugador.Conectado = true;
        jugador.UltimaConexion = DateTime.UtcNow;
        
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "conectado", jugador.Conectado },
            { "ultimaConexion", jugador.UltimaConexion }
        };
        await document.Reference.UpdateAsync(updates);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, jugador.Id),
                new Claim(ClaimTypes.Email, jugador.Correo),
                new Claim(ClaimTypes.Role, jugador.Rol)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        string jwtString = tokenHandler.WriteToken(token);

        var response = new AuthResponse
        {
            Token = jwtString,
            Jugador = MapearAVersionPublica(jugador)
        };

        return ApiResponse<AuthResponse>.Success(response, "Inicio de sesión exitoso.");
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
