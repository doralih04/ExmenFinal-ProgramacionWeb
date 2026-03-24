using Google.Cloud.Firestore;

namespace JuegosTorneosApi.Models.Entities;

[FirestoreData]
public class Juego
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("titulo")]
    public string Titulo { get; set; } = string.Empty;

    [FirestoreProperty("desarrollador")]
    public string Desarrollador { get; set; } = string.Empty;

    [FirestoreProperty("genero")]
    public string Genero { get; set; } = string.Empty;

    [FirestoreProperty("plataformas")]
    public List<string> Plataformas { get; set; } = new();

    [FirestoreProperty("fechaLanzamiento")]
    public DateTime FechaLanzamiento { get; set; }

    [FirestoreProperty("descripcion")]
    public string Descripcion { get; set; } = string.Empty;

    [FirestoreProperty("jugadoresActivos")]
    public int JugadoresActivos { get; set; }

    [FirestoreProperty("torneoActivos")]
    public int TorneoActivos { get; set; }

    [FirestoreProperty("estado")]
    public string Estado { get; set; } = string.Empty;

    [FirestoreProperty("puntuacionPromedio")]
    public double PuntuacionPromedio { get; set; }

    [FirestoreProperty("fechaAgreg")]
    public DateTime FechaAgreg { get; set; }
}
