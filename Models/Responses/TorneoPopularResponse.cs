namespace JuegosTorneosApi.Models.Responses;

public class TorneoPopularResponse
{
    public string TorneoId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Juego { get; set; } = string.Empty;
    public int CantidadInscripciones { get; set; }
    public double PremioTotal { get; set; }
    public string Estado { get; set; } = string.Empty;
}
