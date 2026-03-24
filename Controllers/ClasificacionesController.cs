using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Authorize] // Todos estos endpoints requieren estar autenticados según las reglas previas
public class ClasificacionesController : ControllerBase
{
    private readonly IClasificacionService _clasificacionService;

    public ClasificacionesController(IClasificacionService clasificacionService)
    {
        _clasificacionService = clasificacionService;
    }

    [HttpGet("api/clasificaciones/{juegoId}")]
    public async Task<IActionResult> ObtenerClasificacionesGlobales(string juegoId, [FromQuery] int? minNivel, [FromQuery] int? maxNivel)
    {
        var response = await _clasificacionService.ObtenerClasificacionesPorJuegoAsync(juegoId, minNivel, maxNivel);
        return Ok(response);
    }

    [HttpGet("api/jugador/clasificacion/{juegoId}")]
    public async Task<IActionResult> ObtenerMiClasificacion(string juegoId)
    {
        var jugadorIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(jugadorIdClaim))
        {
            return Unauthorized(ApiResponse<object>.Fail("No se pudo identificar al jugador desde el token autenticado."));
        }

        var response = await _clasificacionService.ObtenerMiClasificacionAsync(juegoId, jugadorIdClaim);

        if (!response.Exito)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}
