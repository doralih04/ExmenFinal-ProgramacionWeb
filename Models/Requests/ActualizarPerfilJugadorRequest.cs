using System.ComponentModel.DataAnnotations;

namespace JuegosTorneosApi.Models.Requests;

public class ActualizarPerfilJugadorRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria")]
    [Range(1, 150, ErrorMessage = "Edad inválida")]
    public int Edad { get; set; }

    [Required(ErrorMessage = "El país es obligatorio")]
    public string Pais { get; set; } = string.Empty;
}
