using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IJuegoService
{
    Task<ApiResponse<JuegoResponse>> CrearJuegoAsync(CrearJuegoRequest request);
}
