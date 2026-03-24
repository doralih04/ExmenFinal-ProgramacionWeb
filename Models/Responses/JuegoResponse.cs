namespace JuegosTorneosApi.Models.Responses;

public class JuegoResponse
{
    public string Id { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Desarrollador { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public List<string> Plataformas { get; set; } = new();
    public DateTime FechaLanzamiento { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public int JugadoresActivos { get; set; }
    public int TorneoActivos { get; set; }
    public string Estado { get; set; } = string.Empty;
    public double PuntuacionPromedio { get; set; }
    public DateTime FechaAgreg { get; set; }
}
