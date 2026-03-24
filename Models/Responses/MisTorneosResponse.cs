namespace JuegosTorneosApi.Models.Responses;

public class MisTorneosResponse
{
    public string ParticipacionId { get; set; } = string.Empty;
    public string EstadoParticipacion { get; set; } = string.Empty;
    public DateTime FechaInscripcion { get; set; }
    public string TorneoId { get; set; } = string.Empty;
    public string NombreTorneo { get; set; } = string.Empty;
    public string JuegoTorneo { get; set; } = string.Empty;
    public DateTime FechaInicioTorneo { get; set; }
    public string EstadoTorneo { get; set; } = string.Empty;
}
