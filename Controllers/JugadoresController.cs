using System.Security.Claims;
using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class JugadoresController : ControllerBase
{
    private readonly IJugadorService _jugadorService;

    public JugadoresController(IJugadorService jugadorService)
    {
        _jugadorService = jugadorService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerJugador(string id)
    {
        var response = await _jugadorService.ObtenerJugadorAsync(id);

        if (!response.Exito)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPut("{id}/perfil")]
    public async Task<IActionResult> ActualizarPerfil(string id, [FromBody] ActualizarPerfilJugadorRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        var currentUserRole = User.FindFirstValue(ClaimTypes.Role) ?? string.Empty;

        var response = await _jugadorService.ActualizarPerfilAsync(id, request, currentUserId, currentUserRole);

        if (!response.Exito)
        {
            if (response.Mensaje.Contains("permiso"))
            {
                return Forbid();
            }
            return NotFound(response);
        }

        return Ok(response);
    }
}
