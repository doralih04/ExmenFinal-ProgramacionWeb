using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Requests;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;

namespace JuegosTorneosApi.Services;

public class JuegoService : IJuegoService
{
    private readonly FirestoreDb _firestoreDb;
    private const string ColeccionJuegos = "juegos";

    public JuegoService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<ApiResponse<JuegoResponse>> CrearJuegoAsync(CrearJuegoRequest request)
    {
        CollectionReference juegosRef = _firestoreDb.Collection(ColeccionJuegos);

        // Validar título único
        Query queryTitulo = juegosRef.WhereEqualTo("titulo", request.Titulo).Limit(1);
        QuerySnapshot snapshotTitulo = await queryTitulo.GetSnapshotAsync();
        
        if (snapshotTitulo.Documents.Count > 0)
        {
            return ApiResponse<JuegoResponse>.Fail("Ya existe un juego con este título.");
        }

        Juego nuevoJuego = new Juego
        {
            Titulo = request.Titulo,
            Desarrollador = request.Desarrollador,
            Genero = request.Genero,
            Plataformas = request.Plataformas,
            FechaLanzamiento = request.FechaLanzamiento.ToUniversalTime(),
            Descripcion = request.Descripcion,
            JugadoresActivos = 0,
            TorneoActivos = 0,
            Estado = request.Estado,
            PuntuacionPromedio = 0.0,
            FechaAgreg = DateTime.UtcNow
        };

        DocumentReference docRef = await juegosRef.AddAsync(nuevoJuego);
        nuevoJuego.Id = docRef.Id;

        var response = MapearAResponse(nuevoJuego);

        return ApiResponse<JuegoResponse>.Success(response, "Juego agregado exitosamente.");
    }

    private JuegoResponse MapearAResponse(Juego juego)
    {
        return new JuegoResponse
        {
            Id = juego.Id,
            Titulo = juego.Titulo,
            Desarrollador = juego.Desarrollador,
            Genero = juego.Genero,
            Plataformas = juego.Plataformas,
            FechaLanzamiento = juego.FechaLanzamiento,
            Descripcion = juego.Descripcion,
            JugadoresActivos = juego.JugadoresActivos,
            TorneoActivos = juego.TorneoActivos,
            Estado = juego.Estado,
            PuntuacionPromedio = juego.PuntuacionPromedio,
            FechaAgreg = juego.FechaAgreg
        };
    }
}
