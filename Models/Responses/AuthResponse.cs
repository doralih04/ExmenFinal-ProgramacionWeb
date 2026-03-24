namespace JuegosTorneosApi.Models.Responses;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;
    public JugadorPublicoResponse Jugador { get; set; } = null!;
}
