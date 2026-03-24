using Google.Cloud.Firestore;

namespace JuegosTorneosApi.Models.Entities;

[FirestoreData]
public class Participacion
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("jugadorId")]
    public string JugadorId { get; set; } = string.Empty;

    [FirestoreProperty("torneoId")]
    public string TorneoId { get; set; } = string.Empty;

    [FirestoreProperty("equipoId")]
    public string EquipoId { get; set; } = string.Empty;

    [FirestoreProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [FirestoreProperty("posicion")]
    public int Posicion { get; set; }

    [FirestoreProperty("puntosObtenidos")]
    public int PuntosObtenidos { get; set; }

    [FirestoreProperty("partidasJugadas")]
    public int PartidasJugadas { get; set; }

    [FirestoreProperty("victorias")]
    public int Victorias { get; set; }

    [FirestoreProperty("derrotas")]
    public int Derrotas { get; set; }

    [FirestoreProperty("fechaInscripcion")]
    public DateTime FechaInscripcion { get; set; }

    [FirestoreProperty("fechaEliminacion")]
    public DateTime? FechaEliminacion { get; set; }

    [FirestoreProperty("estadisticas")]
    public EstadisticasParticipacion Estadisticas { get; set; } = new();

    [FirestoreProperty("penalizaciones")]
    public int Penalizaciones { get; set; }

    [FirestoreProperty("pagado")]
    public bool Pagado { get; set; }
}
