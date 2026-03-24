using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "organizador,admin")]
public class TorneosController : ControllerBase
{
    private readonly ITorneoService _torneoService;

    public TorneosController(ITorneoService torneoService)
    {
        _torneoService = torneoService;
    }

    [HttpPost]
    public async Task<IActionResult> CrearTorneo([FromBody] CrearTorneoRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _torneoService.CrearTorneoAsync(request);

        if (!response.Exito)
        {
            return Conflict(response);
        }

        return Created("", response);
    }
}
