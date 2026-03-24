using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "admin")]
public class JuegosController : ControllerBase
{
    private readonly IJuegoService _juegoService;

    public JuegosController(IJuegoService juegoService)
    {
        _juegoService = juegoService;
    }

    [HttpPost]
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
}
