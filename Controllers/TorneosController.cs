using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TorneosController : ControllerBase
{
    private readonly ITorneoService _torneoService;

    public TorneosController(ITorneoService torneoService)
    {
        _torneoService = torneoService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ObtenerTorneos([FromQuery] string? juego, [FromQuery] string? estado, [FromQuery] string? formato)
    {
        var response = await _torneoService.ObtenerTorneosAsync(juego, estado, formato);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> ObtenerTorneo(string id)
    {
        var response = await _torneoService.ObtenerTorneoAsync(id);
        
        if (!response.Exito)
            return NotFound(response);

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> CrearTorneo([FromBody] CrearTorneoRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _torneoService.CrearTorneoAsync(request);

        if (!response.Exito)
            return Conflict(response);

        return Created("", response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> ActualizarTorneo(string id, [FromBody] ActualizarTorneoRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _torneoService.ActualizarTorneoAsync(id, request);

        if (!response.Exito)
        {
            if (response.Mensaje.Contains("juego referenciado")) return Conflict(response);
            if (response.Mensaje.Contains("reducir")) return BadRequest(response);
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPatch("{id}/estado")]
    [Authorize(Roles = "organizador,admin")]
    public async Task<IActionResult> CambiarEstadoTorneo(string id, [FromBody] CambiarEstadoTorneoRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _torneoService.CambiarEstadoTorneoAsync(id, request);

        if (!response.Exito)
            return NotFound(response);

        return Ok(response);
    }
}
