using System.ComponentModel.DataAnnotations;

namespace JuegosTorneosApi.Models.Requests;

public class RegistroRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio")]
    public string Apellido { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio")]
    [EmailAddress(ErrorMessage = "Formato de correo inválido")]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    public string Contrasena { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La edad es obligatoria")]
    [Range(1, 150, ErrorMessage = "Edad inválida")]
    public int Edad { get; set; }

    [Required(ErrorMessage = "El país es obligatorio")]
    public string Pais { get; set; } = string.Empty;
}
