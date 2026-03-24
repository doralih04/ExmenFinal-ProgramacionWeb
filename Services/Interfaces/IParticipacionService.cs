using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IParticipacionService
{
    Task<ApiResponse<ParticipacionResponse>> InscribirseAsync(string torneoId, string jugadorId, InscribirseTorneoRequest request);
}
