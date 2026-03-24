using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IClasificacionService
{
    Task<ApiResponse<List<ClasificacionJuegoResponse>>> ObtenerClasificacionesPorJuegoAsync(string juegoId, int? minNivel, int? maxNivel);
    Task<ApiResponse<MiClasificacionResponse>> ObtenerMiClasificacionAsync(string juegoId, string jugadorId);
}
