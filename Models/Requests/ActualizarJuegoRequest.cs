using System.ComponentModel.DataAnnotations;

namespace JuegosTorneosApi.Models.Requests;

public class ActualizarJuegoRequest
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

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [MinLength(20, ErrorMessage = "La descripción debe tener como mínimo 20 caracteres")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El estado es obligatorio (ej. disponible, inactivo)")]
    public string Estado { get; set; } = string.Empty;
}
