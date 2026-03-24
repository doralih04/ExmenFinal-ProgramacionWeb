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

        return ApiResponse<JuegoResponse>.Success(MapearAResponse(nuevoJuego), "Juego agregado exitosamente.");
    }

    public async Task<ApiResponse<List<JuegoResponse>>> ObtenerJuegosDisponiblesAsync(string? genero, string? plataforma, string? desarrollador)
    {
        CollectionReference juegosRef = _firestoreDb.Collection(ColeccionJuegos);
        
        // Estado por defecto examinado como "disponible" o "Disponible" (usando "disponible" como la validación base)
        Query query = juegosRef.WhereIn("estado", new[] { "disponible", "Disponible" });

        if (!string.IsNullOrEmpty(genero))
        {
            query = query.WhereEqualTo("genero", genero);
        }

        if (!string.IsNullOrEmpty(desarrollador))
        {
            query = query.WhereEqualTo("desarrollador", desarrollador);
        }

        if (!string.IsNullOrEmpty(plataforma))
        {
            query = query.WhereArrayContains("plataformas", plataforma);
        }

        QuerySnapshot snapshot = await query.GetSnapshotAsync();
        List<JuegoResponse> juegos = new List<JuegoResponse>();

        foreach (var doc in snapshot.Documents)
        {
            if (doc.Exists)
            {
                Juego j = doc.ConvertTo<Juego>();
                j.Id = doc.Id;
                juegos.Add(MapearAResponse(j));
            }
        }

        return ApiResponse<List<JuegoResponse>>.Success(juegos);
    }

    public async Task<ApiResponse<JuegoResponse>> ObtenerJuegoAsync(string id)
    {
        DocumentReference docRef = _firestoreDb.Collection(ColeccionJuegos).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return ApiResponse<JuegoResponse>.Fail("Juego no encontrado.");
        }

        Juego juego = snapshot.ConvertTo<Juego>();
        juego.Id = snapshot.Id;

        return ApiResponse<JuegoResponse>.Success(MapearAResponse(juego));
    }

    public async Task<ApiResponse<JuegoResponse>> ActualizarJuegoAsync(string id, ActualizarJuegoRequest request)
    {
        DocumentReference docRef = _firestoreDb.Collection(ColeccionJuegos).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return ApiResponse<JuegoResponse>.Fail("Juego no encontrado.");
        }

        Juego juegoExistente = snapshot.ConvertTo<Juego>();

        // Validar título único si ha cambiado
        if (juegoExistente.Titulo != request.Titulo)
        {
            Query queryTitulo = _firestoreDb.Collection(ColeccionJuegos).WhereEqualTo("titulo", request.Titulo).Limit(1);
            QuerySnapshot snapshotTitulo = await queryTitulo.GetSnapshotAsync();
            
            if (snapshotTitulo.Documents.Count > 0)
            {
                return ApiResponse<JuegoResponse>.Fail("Ya existe otro juego con este título.");
            }
        }

        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "titulo", request.Titulo },
            { "desarrollador", request.Desarrollador },
            { "genero", request.Genero },
            { "plataformas", request.Plataformas },
            { "descripcion", request.Descripcion },
            { "estado", request.Estado }
        };

        await docRef.UpdateAsync(updates);

        DocumentSnapshot updatedSnapshot = await docRef.GetSnapshotAsync();
        Juego juegoUpdated = updatedSnapshot.ConvertTo<Juego>();
        juegoUpdated.Id = updatedSnapshot.Id;

        return ApiResponse<JuegoResponse>.Success(MapearAResponse(juegoUpdated), "Juego actualizado exitosamente.");
    }

    public async Task<ApiResponse<string>> EliminarJuegoAsync(string id)
    {
        DocumentReference docRef = _firestoreDb.Collection(ColeccionJuegos).Document(id);
        DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

        if (!snapshot.Exists)
        {
            return ApiResponse<string>.Fail("Juego no encontrado.");
        }

        // Eliminación física
        await docRef.DeleteAsync();

        return ApiResponse<string>.Success(id, "Juego eliminado permanentemente de Firestore.");
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
