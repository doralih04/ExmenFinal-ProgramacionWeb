namespace JuegosTorneosApi.Models.Responses;

public class MiDesempenoResponse
{
    public int NivelActual { get; set; }
    public int PosicionRanking { get; set; }
    public double ProgresoSiguienteNivel { get; set; } // porcentaje 0-100
    public double RatioVictoria { get; set; }
    public int RachaActual { get; set; }
    public int MedallasOro { get; set; }
    public int MedallasPlata { get; set; }
    public int MedallasBronce { get; set; }
    public List<string> MejoresTorneos { get; set; } = new(); // top 3 torneoId por puntaje
}
