using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;

namespace JuegosTorneosApi.Services;

public class ClasificacionService : IClasificacionService
{
    private readonly FirestoreDb _firestoreDb;

    public ClasificacionService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    public async Task<ApiResponse<List<ClasificacionJuegoResponse>>> ObtenerClasificacionesPorJuegoAsync(string juegoId, int? minNivel, int? maxNivel)
    {
        // 1. Obtener todas las clasificaciones de este juego
        Query query = _firestoreDb.Collection("clasificaciones").WhereEqualTo("juegoId", juegoId);
        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        var clasificaciones = snapshot.Documents
            .Where(d => d.Exists)
            .Select(d => d.ConvertTo<Clasificacion>())
            .ToList();

        // 2. Aplicar filtros de nivel en memoria
        // (Hacerlo en memoria evita crashes por falta de Índices Compuestos en Firestore cuando se mezcla WhereGreaterThan con OrderBy escalar)
        if (minNivel.HasValue)
            clasificaciones = clasificaciones.Where(c => c.NivelJuego >= minNivel.Value).ToList();
        
        if (maxNivel.HasValue)
            clasificaciones = clasificaciones.Where(c => c.NivelJuego <= maxNivel.Value).ToList();

        // 3. Ordenar por posicion ascendente y aplicar Paginación (máximo 50)
        var top50 = clasificaciones.OrderBy(c => c.Posicion).Take(50).ToList();

        List<ClasificacionJuegoResponse> resultado = new List<ClasificacionJuegoResponse>();

        // 4. Join lógico para hidratar el nombre del jugador
        foreach (var c in top50)
        {
            string nombreJugador = "Desconocido";
            if (!string.IsNullOrEmpty(c.JugadorId))
            {
                DocumentSnapshot jugDoc = await _firestoreDb.Collection("jugadores").Document(c.JugadorId).GetSnapshotAsync();
                if (jugDoc.Exists)
                {
                    // Si el jugador tiene "nombreUsuario" (del Commit 1) lo usamos
                    nombreJugador = jugDoc.TryGetValue("nombreUsuario", out string alias) ? alias : 
                                    (jugDoc.TryGetValue("nombre", out string nom) ? nom : "Desconocido");
                }
            }

            resultado.Add(new ClasificacionJuegoResponse
            {
                Posicion = c.Posicion,
                NombreJugador = nombreJugador,
                Puntos = c.PuntosJuego,
                Nivel = c.NivelJuego,
                RatioVictoria = c.RatioVictoria,
                TotalPartidas = c.TotalPartidas,
                RachaActual = c.Racha
            });
        }

        return ApiResponse<List<ClasificacionJuegoResponse>>.Success(resultado);
    }

    public async Task<ApiResponse<MiClasificacionResponse>> ObtenerMiClasificacionAsync(string juegoId, string jugadorId)
    {
        Query query = _firestoreDb.Collection("clasificaciones")
            .WhereEqualTo("juegoId", juegoId)
            .WhereEqualTo("jugadorId", jugadorId)
            .Limit(1);

        QuerySnapshot snapshot = await query.GetSnapshotAsync();

        if (snapshot.Documents.Count == 0 || !snapshot.Documents[0].Exists)
        {
            return ApiResponse<MiClasificacionResponse>.Fail("No se encontraron clasificaciones para este jugador en este juego.");
        }

        Clasificacion miClasificacion = snapshot.Documents[0].ConvertTo<Clasificacion>();

        MiClasificacionResponse response = new MiClasificacionResponse
        {
            Rank = miClasificacion.Posicion,
            Puntos = miClasificacion.PuntosJuego,
            Nivel = miClasificacion.NivelJuego,
            MedallasOro = miClasificacion.MedallasOro,
            MedallasPlata = miClasificacion.MedallaPlata,
            MedallasBronce = miClasificacion.MedallaBronce,
            LogrosDesbloqueados = miClasificacion.Logros ?? new List<string>()
        };

        return ApiResponse<MiClasificacionResponse>.Success(response);
    }
}
