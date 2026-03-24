using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface ITorneoService
{
    Task<ApiResponse<TorneoResponse>> CrearTorneoAsync(CrearTorneoRequest request);
}
