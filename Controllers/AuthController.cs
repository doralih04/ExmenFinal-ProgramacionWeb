using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JuegosTorneosApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _authService.RegistroAsync(request);

        if (!response.Exito)
        {
            return Conflict(response);
        }

        return Created("", response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(ApiResponse<object>.Fail("Errores de validación", errores));
        }

        var response = await _authService.LoginAsync(request);

        if (!response.Exito)
        {
            return Unauthorized(response);
        }

        return Ok(response);
    }
}
