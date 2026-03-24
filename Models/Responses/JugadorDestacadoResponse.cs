namespace JuegosTorneosApi.Models.Responses;

public class JugadorDestacadoResponse
{
    public string JugadorId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int PuntosGlobales { get; set; }
    public int TorneosGanados { get; set; }
    public int CantidadJuegos { get; set; }
}
