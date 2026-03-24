using System.ComponentModel.DataAnnotations;

namespace JuegosTorneosApi.Models.Requests;

public class FormatoTorneoValidoAttribute : ValidationAttribute
{
    private static readonly List<string> FormatosPermitidos = new() { "individual", "equipos", "royale" };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is string formato && !string.IsNullOrEmpty(formato))
        {
            if (!FormatosPermitidos.Contains(formato.ToLowerInvariant()))
            {
                return new ValidationResult($"Formato inválido. Se acepta: {string.Join(", ", FormatosPermitidos)}.");
            }
        }
        return ValidationResult.Success;
    }
}

public class FechasTorneoValidasAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var request = (CrearTorneoRequest)validationContext.ObjectInstance;

        if (request.FechaInicio <= DateTime.UtcNow)
        {
            return new ValidationResult("La fecha de inicio debe ser futura.");
        }

        if (request.FechaLimiteInscripcion >= request.FechaInicio)
        {
            return new ValidationResult("La fecha límite de inscripción debe ser anterior a la fecha de inicio.");
        }

        if (request.FechaFin <= request.FechaInicio)
        {
            return new ValidationResult("La fecha de fin debe ser posterior a la fecha de inicio.");
        }

        return ValidationResult.Success;
    }
}

[FechasTorneoValidas]
public class CrearTorneoRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo juego (ID) es obligatorio.")]
    public string Juego { get; set; } = string.Empty;

    [Required(ErrorMessage = "El organizador (ID) es obligatorio.")]
    public string Organizador { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    public string Estado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El formato es obligatorio.")]
    [FormatoTorneoValido]
    public string Formato { get; set; } = string.Empty;

    [Required(ErrorMessage = "El máximo de participantes es obligatorio y mayor a 2.")]
    [Range(3, int.MaxValue, ErrorMessage = "Debe haber al menos 3 participantes máximo.")]
    public int MaxParticipantes { get; set; }

    public double PrecioInscripcion { get; set; }
    public double PremioTotal { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "La fecha de fin es obligatoria.")]
    public DateTime FechaFin { get; set; }

    [Required(ErrorMessage = "La fecha límite de inscripción es obligatoria.")]
    public DateTime FechaLimiteInscripcion { get; set; }

    public int MinNivel { get; set; }
    public int MaxNivel { get; set; }
    public bool RequiereEquipo { get; set; }
    public int TamanioEquipo { get; set; }
    public string ReglasModificadas { get; set; } = string.Empty;
}
