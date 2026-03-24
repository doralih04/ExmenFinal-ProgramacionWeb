using System.ComponentModel.DataAnnotations;

namespace JuegosTorneosApi.Models.Requests;

[FechasTorneoValidas] // Reutilizando la validación creada anteriormente
public class ActualizarTorneoRequest
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El campo juego (ID) es obligatorio.")]
    public string Juego { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; } = string.Empty;

    [Required(ErrorMessage = "El formato es obligatorio.")]
    [FormatoTorneoValido] // Reutilizando la validación
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
