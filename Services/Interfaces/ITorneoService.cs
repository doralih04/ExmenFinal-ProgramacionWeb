using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface ITorneoService
{
    Task<ApiResponse<TorneoResponse>> CrearTorneoAsync(CrearTorneoRequest request);
    Task<ApiResponse<List<TorneoResponse>>> ObtenerTorneosAsync(string? juego, string? estado, string? formato);
    Task<ApiResponse<TorneoResponse>> ObtenerTorneoAsync(string id);
    Task<ApiResponse<TorneoResponse>> ActualizarTorneoAsync(string id, ActualizarTorneoRequest request);
    Task<ApiResponse<TorneoResponse>> CambiarEstadoTorneoAsync(string id, CambiarEstadoTorneoRequest request);
}
