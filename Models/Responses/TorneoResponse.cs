namespace JuegosTorneosApi.Models.Responses;

public class TorneoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Juego { get; set; } = string.Empty;
    public string Organizador { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Formato { get; set; } = string.Empty;
    public int MaxParticipantes { get; set; }
    public int ParticipantesActuales { get; set; }
    public double PrecioInscripcion { get; set; }
    public double PremioTotal { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime FechaLimiteInscripcion { get; set; }
    public int MinNivel { get; set; }
    public int MaxNivel { get; set; }
    public bool RequiereEquipo { get; set; }
    public int TamanioEquipo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public string ReglasModificadas { get; set; } = string.Empty;
}
