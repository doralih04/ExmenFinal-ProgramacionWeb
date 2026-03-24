using Google.Cloud.Firestore;

namespace JuegosTorneosApi.Models.Entities;

[FirestoreData]
public class EstadisticasParticipacion
{
    [FirestoreProperty("asesinatos")]
    public int Asesinatos { get; set; }

    [FirestoreProperty("muertes")]
    public int Muertes { get; set; }

    [FirestoreProperty("asistencias")]
    public int Asistencias { get; set; }

    [FirestoreProperty("dañoCausado")]
    public double DañoCausado { get; set; }
}
