namespace JuegosTorneosApi.Models.Responses;

public class MiClasificacionResponse
{
    public int Rank { get; set; }
    public int Puntos { get; set; }
    public int Nivel { get; set; }
    
    // Agrupación visual de medallas
    public int MedallasOro { get; set; }
    public int MedallasPlata { get; set; }
    public int MedallasBronce { get; set; }

    public List<string> LogrosDesbloqueados { get; set; } = new();
}
