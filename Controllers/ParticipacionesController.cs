using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
public class ParticipacionesController : ControllerBase
{
    private readonly IParticipacionService _participacionService;

    public ParticipacionesController(IParticipacionService participacionService)
    {
        _participacionService = participacionService;
    }

    [HttpGet("/api/jugador/mis-torneos")]
    [Authorize]
    public async Task<IActionResult> ObtenerMisTorneos()
    {
        var jugadorIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(jugadorIdClaim))
        {
            return Unauthorized(ApiResponse<object>.Fail("No se pudo identificar al jugador desde el token autenticado."));
        }

        var response = await _participacionService.ObtenerMisTorneosAsync(jugadorIdClaim);
        return Ok(response);
    }

    [HttpPatch("api/participaciones/{id}/abandonar")]
    [Authorize]
    public async Task<IActionResult> AbandonarTorneo(string id)
    {
        var jugadorIdClaim = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(jugadorIdClaim))
        {
            return Unauthorized(ApiResponse<object>.Fail("No se pudo identificar al jugador desde el token autenticado."));
        }

        var response = await _participacionService.AbandonarTorneoAsync(id, jugadorIdClaim);

        if (!response.Exito)
        {
            if (response.Mensaje.Contains("no existe")) return NotFound(response);
            if (response.Mensaje.Contains("permiso")) return StatusCode(StatusCodes.Status403Forbidden, response);
            return BadRequest(response); // Casos como torneo ya en progreso o ya abandonado
        }

        return Ok(response);
    }
}
