using System.ComponentModel.DataAnnotations;

namespace JuegosTorneosApi.Models.Requests;

public class EstadoTorneoValidoAttribute : ValidationAttribute
{
    private static readonly List<string> EstadosPermitidos = new() { "próximo", "en progreso", "finalizado", "cancelado" };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string estado && !string.IsNullOrEmpty(estado))
        {
            if (!EstadosPermitidos.Contains(estado.ToLowerInvariant()))
            {
                return new ValidationResult($"Estado inválido. Se acepta: {string.Join(", ", EstadosPermitidos)}.");
            }
        }
        return ValidationResult.Success;
    }
}

public class CambiarEstadoTorneoRequest
{
    [Required(ErrorMessage = "El estado es obligatorio.")]
    [EstadoTorneoValido]
    public string Estado { get; set; } = string.Empty;
}
