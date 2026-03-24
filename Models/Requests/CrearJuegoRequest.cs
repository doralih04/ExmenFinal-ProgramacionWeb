using System.ComponentModel.DataAnnotations;

namespace JuegosTorneosApi.Models.Requests;

public class PlataformasValidasAttribute : ValidationAttribute
{
    private static readonly List<string> PlataformasPermitidas = new() { "PC", "PS5", "Xbox", "Switch" };

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is List<string> plataformas && plataformas.Any())
        {
            var invalidas = plataformas.Where(p => !PlataformasPermitidas.Contains(p)).ToList();
            if (invalidas.Any())
            {
                return new ValidationResult($"Plataformas no válidas: {string.Join(", ", invalidas)}. Se aceptan solo: {string.Join(", ", PlataformasPermitidas)}.");
            }
        }
        else
        {
            return new ValidationResult("Debe listar al menos una plataforma válida.");
        }

        return ValidationResult.Success;
    }
}

public class CrearJuegoRequest
{
    [Required(ErrorMessage = "El título es obligatorio")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El desarrollador es obligatorio")]
    public string Desarrollador { get; set; } = string.Empty;

    [Required(ErrorMessage = "El género es obligatorio")]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "Las plataformas son obligatorias")]
    [PlataformasValidas]
    public List<string> Plataformas { get; set; } = new();

    [Required(ErrorMessage = "La fecha de lanzamiento es obligatoria")]
    public DateTime FechaLanzamiento { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MinLength(20, ErrorMessage = "La descripción debe tener como mínimo 20 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es obligatorio")]
    public string Estado { get; set; } = string.Empty;
}
