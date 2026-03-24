using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IJuegoService
{
    Task<ApiResponse<JuegoResponse>> CrearJuegoAsync(CrearJuegoRequest request);
    Task<ApiResponse<List<JuegoResponse>>> ObtenerJuegosDisponiblesAsync(string? genero, string? plataforma, string? desarrollador);
    Task<ApiResponse<JuegoResponse>> ObtenerJuegoAsync(string id);
    Task<ApiResponse<JuegoResponse>> ActualizarJuegoAsync(string id, ActualizarJuegoRequest request);
    Task<ApiResponse<string>> EliminarJuegoAsync(string id);
}
