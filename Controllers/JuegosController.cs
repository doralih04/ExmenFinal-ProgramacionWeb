using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Requerido para todos por defecto
public class JuegosController : ControllerBase
{
    private readonly IJuegoService _juegoService;

    public JuegosController(IJuegoService juegoService)
    {
        _juegoService = juegoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerJuegos([FromQuery] string? genero, [FromQuery] string? plataforma, [FromQuery] string? desarrollador)
    {
        var response = await _juegoService.ObtenerJuegosDisponiblesAsync(genero, plataforma, desarrollador);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerJuego(string id)
    {
        var response = await _juegoService.ObtenerJuegoAsync(id);

        if (!response.Exito)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CrearJuego([FromBody] CrearJuegoRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _juegoService.CrearJuegoAsync(request);

        if (!response.Exito)
        {
            return Conflict(response);
        }

        return Created("", response);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> ActualizarJuego(string id, [FromBody] ActualizarJuegoRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _juegoService.ActualizarJuegoAsync(id, request);

        if (!response.Exito)
        {
            if (response.Mensaje.Contains("título"))
                return Conflict(response);
            
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> EliminarJuego(string id)
    {
        var response = await _juegoService.EliminarJuegoAsync(id);

        if (!response.Exito)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
}
