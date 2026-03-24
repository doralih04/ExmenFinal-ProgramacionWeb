using JuegosTorneosApi.Models.Responses;

namespace JuegosTorneosApi.Services.Interfaces;

public interface IReporteService
{
    Task<ApiResponse<List<TorneoPopularResponse>>> ObtenerTorneosPopularesAsync();
    Task<ApiResponse<List<JugadorDestacadoResponse>>> ObtenerJugadoresDestacadosAsync();
    Task<ApiResponse<MiDesempenoResponse>> ObtenerMiDesempenoAsync(string juegoId, string jugadorId);
    Task<ApiResponse<TendenciasResponse>> ObtenerTendenciasAsync();
}
