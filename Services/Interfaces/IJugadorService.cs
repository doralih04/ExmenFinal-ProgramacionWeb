using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IJugadorService
{
    Task<ApiResponse<JugadorPublicoResponse>> ObtenerJugadorAsync(string id);
    Task<ApiResponse<JugadorPublicoResponse>> ActualizarPerfilAsync(string id, ActualizarPerfilJugadorRequest request, string currentUserId, string currentUserRole);
}
