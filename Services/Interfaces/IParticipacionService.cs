using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IParticipacionService
{
    Task<ApiResponse<ParticipacionResponse>> InscribirseAsync(string torneoId, string jugadorId, InscribirseTorneoRequest request);
    Task<ApiResponse<List<MisTorneosResponse>>> ObtenerMisTorneosAsync(string jugadorId);
    Task<ApiResponse<ParticipacionResponse>> AbandonarTorneoAsync(string participacionId, string jugadorId);
}
