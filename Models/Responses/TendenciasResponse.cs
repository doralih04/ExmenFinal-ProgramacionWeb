namespace JuegosTorneosApi.Models.Responses;

public class TendenciasResponse
{
    public List<string> JuegosMasPopulares { get; set; } = new();          // top 5 títulos
    public List<string> GenerosMasTorneos { get; set; } = new();           // géneros con más torneos activos
    public string HoraPicoActividad { get; set; } = string.Empty;          // hora estimada de mayor actividad
}
