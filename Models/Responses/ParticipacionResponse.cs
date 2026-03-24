using JuegosTorneosApi.Models.Entities;

namespace JuegosTorneosApi.Models.Responses;

public class ParticipacionResponse
{
    public string Id { get; set; } = string.Empty;
    public string JugadorId { get; set; } = string.Empty;
    public string TorneoId { get; set; } = string.Empty;
    public string EquipoId { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public int Posicion { get; set; }
    public int PuntosObtenidos { get; set; }
    public int PartidasJugadas { get; set; }
    public int Victorias { get; set; }
    public int Derrotas { get; set; }
    public DateTime FechaInscripcion { get; set; }
    public DateTime? FechaEliminacion { get; set; }
    public EstadisticasParticipacion Estadisticas { get; set; } = new();
    public int Penalizaciones { get; set; }
    public bool Pagado { get; set; }
}
