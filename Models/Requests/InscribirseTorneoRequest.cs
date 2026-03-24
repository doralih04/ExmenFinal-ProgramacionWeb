namespace JuegosTorneosApi.Models.Requests;

public class InscribirseTorneoRequest
{
    public string EquipoId { get; set; } = string.Empty;
    public bool Pagado { get; set; }
}
