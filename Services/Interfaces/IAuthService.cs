using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<JugadorPublicoResponse>> RegistroAsync(RegistroRequest request);
}
