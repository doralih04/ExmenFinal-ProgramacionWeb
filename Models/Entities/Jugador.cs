using Google.Cloud.Firestore;

namespace JuegosTorneosApi.Models.Entities;

[FirestoreData]
public class Jugador
{
    [FirestoreDocumentId]
    public string Id { get; set; } = string.Empty;

    [FirestoreProperty("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [FirestoreProperty("apellido")]
    public string Apellido { get; set; } = string.Empty;

    [FirestoreProperty("correo")]
    public string Correo { get; set; } = string.Empty;

    [FirestoreProperty("contrasena")]
    public string Contrasena { get; set; } = string.Empty;

    [FirestoreProperty("nombreUsuario")]
    public string NombreUsuario { get; set; } = string.Empty;

    [FirestoreProperty("edad")]
    public int Edad { get; set; }

    [FirestoreProperty("pais")]
    public string Pais { get; set; } = string.Empty;

    [FirestoreProperty("rol")]
    public string Rol { get; set; } = string.Empty;

    [FirestoreProperty("activo")]
    public bool Activo { get; set; }

    [FirestoreProperty("puntosGlobales")]
    public int PuntosGlobales { get; set; }

    [FirestoreProperty("torneosGanados")]
    public int TorneosGanados { get; set; }

    [FirestoreProperty("fechaRegistro")]
    public DateTime FechaRegistro { get; set; }

    [FirestoreProperty("conectado")]
    public bool Conectado { get; set; }

    [FirestoreProperty("ultimaConexion")]
    public DateTime? UltimaConexion { get; set; }
}
