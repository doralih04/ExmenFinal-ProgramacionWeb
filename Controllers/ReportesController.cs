using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Route("api/reportes")]
public class ReportesController : ControllerBase
{
    private readonly IReporteService _reporteService;

    public ReportesController(IReporteService reporteService)
    {
        _reporteService = reporteService;
    }

    [HttpGet("torneos-populares")]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> ObtenerTorneosPopulares()
    {
        var response = await _reporteService.ObtenerTorneosPopularesAsync();
        return Ok(response);
    }

    [HttpGet("jugadores-destacados")]
    [Authorize]
    public async Task<IActionResult> ObtenerJugadoresDestacados()
    {
        var response = await _reporteService.ObtenerJugadoresDestacadosAsync();
        return Ok(response);
    }

    [HttpGet("mi-desempeno/{juegoId}")]
    [Authorize]
    public async Task<IActionResult> ObtenerMiDesempeno(string juegoId)
    {
        var jugadorIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(jugadorIdClaim))
        {
            return Unauthorized(ApiResponse<object>.Fail("No se pudo identificar al jugador desde el token autenticado."));
        }

        var response = await _reporteService.ObtenerMiDesempenoAsync(juegoId, jugadorIdClaim);

        if (!response.Exito)
            return NotFound(response);

        return Ok(response);
    }

    [HttpGet("tendencias")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ObtenerTendencias()
    {
        var response = await _reporteService.ObtenerTendenciasAsync();
        return Ok(response);
    }
}
