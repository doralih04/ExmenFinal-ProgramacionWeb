using Google.Cloud.Firestore;
using JuegosTorneosApi.Models.Entities;
using JuegosTorneosApi.Models.Responses;
using JuegosTorneosApi.Services.Interfaces;

namespace JuegosTorneosApi.Services;

public class ReporteService : IReporteService
{
    private readonly FirestoreDb _firestoreDb;

    public ReporteService(FirestoreDb firestoreDb)
    {
        _firestoreDb = firestoreDb;
    }

    // ─────────────────────────────────────────────
    // GET /api/reportes/torneos-populares
    // ─────────────────────────────────────────────
    public async Task<ApiResponse<List<TorneoPopularResponse>>> ObtenerTorneosPopularesAsync()
    {
        DateTime hace30Dias = DateTime.UtcNow.AddDays(-30);

        // 1. Traer participaciones de los últimos 30 días
        Query partQuery = _firestoreDb.Collection("participaciones")
            .WhereGreaterThanOrEqualTo("fechaInscripcion", hace30Dias);
        QuerySnapshot partSnapshot = await partQuery.GetSnapshotAsync();

        // 2. Agrupar y contar inscripciones por torneoId
        var conteosPorTorneo = partSnapshot.Documents
            .Where(d => d.Exists)
            .Select(d => d.ConvertTo<Participacion>())
            .GroupBy(p => p.TorneoId)
            .Select(g => new { TorneoId = g.Key, Inscripciones = g.Count() })
            .OrderByDescending(x => x.Inscripciones)
            .Take(10)
            .ToList();

        List<TorneoPopularResponse> resultado = new();

        // 3. Enriquecer con datos del torneo
        foreach (var item in conteosPorTorneo)
        {
            DocumentSnapshot torneoDoc = await _firestoreDb.Collection("torneos").Document(item.TorneoId).GetSnapshotAsync();
            if (torneoDoc.Exists)
            {
                Torneo t = torneoDoc.ConvertTo<Torneo>();
                resultado.Add(new TorneoPopularResponse
                {
                    TorneoId = torneoDoc.Id,
                    Nombre = t.Nombre,
                    Juego = t.Juego,
                    CantidadInscripciones = item.Inscripciones,
                    PremioTotal = t.PremioTotal,
                    Estado = t.Estado
                });
            }
        }

        return ApiResponse<List<TorneoPopularResponse>>.Success(resultado);
    }

    // ─────────────────────────────────────────────
    // GET /api/reportes/jugadores-destacados
    // ─────────────────────────────────────────────
    public async Task<ApiResponse<List<JugadorDestacadoResponse>>> ObtenerJugadoresDestacadosAsync()
    {
        QuerySnapshot jugSnapshot = await _firestoreDb.Collection("jugadores").GetSnapshotAsync();

        var jugadores = jugSnapshot.Documents
            .Where(d => d.Exists)
            .Select(d =>
            {
                d.TryGetValue("puntosGlobales", out int puntos);
                d.TryGetValue("torneosGanados", out int torneos);
                d.TryGetValue("nombre", out string nombre);
                return new { Id = d.Id, Nombre = nombre ?? "", PuntosGlobales = puntos, TorneosGanados = torneos };
            })
            .OrderByDescending(j => j.PuntosGlobales)
            .Take(20)
            .ToList();

        List<JugadorDestacadoResponse> resultado = new();

        foreach (var jug in jugadores)
        {
            // Contar en cuántos juegos distintos aparece en clasificaciones
            Query clasifQuery = _firestoreDb.Collection("clasificaciones").WhereEqualTo("jugadorId", jug.Id);
            QuerySnapshot clasifSnapshot = await clasifQuery.GetSnapshotAsync();
            int cantidadJuegos = clasifSnapshot.Documents
                .Where(d => d.Exists)
                .Select(d => d.ConvertTo<Clasificacion>().JuegoId)
                .Distinct()
                .Count();

            resultado.Add(new JugadorDestacadoResponse
            {
                JugadorId = jug.Id,
                Nombre = jug.Nombre,
                PuntosGlobales = jug.PuntosGlobales,
                TorneosGanados = jug.TorneosGanados,
                CantidadJuegos = cantidadJuegos
            });
        }

        return ApiResponse<List<JugadorDestacadoResponse>>.Success(resultado);
    }

    // ─────────────────────────────────────────────
    // GET /api/reportes/mi-desempeno/{juegoId}
    // ─────────────────────────────────────────────
    public async Task<ApiResponse<MiDesempenoResponse>> ObtenerMiDesempenoAsync(string juegoId, string jugadorId)
    {
        // 1. Obtener clasificación del jugador en ese juego
        Query clasifQuery = _firestoreDb.Collection("clasificaciones")
            .WhereEqualTo("juegoId", juegoId)
            .WhereEqualTo("jugadorId", jugadorId)
            .Limit(1);
        QuerySnapshot clasifSnapshot = await clasifQuery.GetSnapshotAsync();

        if (clasifSnapshot.Documents.Count == 0 || !clasifSnapshot.Documents[0].Exists)
        {
            return ApiResponse<MiDesempenoResponse>.Fail("No se encontró clasificación para este jugador en este juego.");
        }

        Clasificacion c = clasifSnapshot.Documents[0].ConvertTo<Clasificacion>();

        // 2. Calcular progreso hacia siguiente nivel (puntos del nivel actual en escala 0-1000 por nivel)
        double progresoSiguienteNivel = Math.Round((c.PuntosJuego % 1000) / 10.0, 1); // escala simple

        // 3. Top 3 participaciones por puntosObtenidos en torneos de este juego 
        Query partQuery = _firestoreDb.Collection("participaciones")
            .WhereEqualTo("jugadorId", jugadorId);
        QuerySnapshot partSnapshot = await partQuery.GetSnapshotAsync();

        // Filtrar participaciones en torneos que correspondan a este juego
        var mejoresTorneos = new List<string>();
        var partsFiltradas = partSnapshot.Documents
            .Where(d => d.Exists)
            .Select(d => d.ConvertTo<Participacion>())
            .OrderByDescending(p => p.PuntosObtenidos)
            .Take(3)
            .ToList();
        mejoresTorneos = partsFiltradas.Select(p => p.TorneoId).ToList();

        MiDesempenoResponse response = new()
        {
            NivelActual = c.NivelJuego,
            PosicionRanking = c.Posicion,
            ProgresoSiguienteNivel = progresoSiguienteNivel,
            RatioVictoria = c.RatioVictoria,
            RachaActual = c.Racha,
            MedallasOro = c.MedallasOro,
            MedallasPlata = c.MedallaPlata,
            MedallasBronce = c.MedallaBronce,
            MejoresTorneos = mejoresTorneos
        };

        return ApiResponse<MiDesempenoResponse>.Success(response);
    }

    // ─────────────────────────────────────────────
    // GET /api/reportes/tendencias
    // ─────────────────────────────────────────────
    public async Task<ApiResponse<TendenciasResponse>> ObtenerTendenciasAsync()
    {
        // 1. Top 5 juegos más populares (por jugadoresActivos)
        QuerySnapshot juegosSnapshot = await _firestoreDb.Collection("juegos").GetSnapshotAsync();
        var top5Juegos = juegosSnapshot.Documents
            .Where(d => d.Exists)
            .Select(d =>
            {
                d.TryGetValue("titulo", out string titulo);
                d.TryGetValue("jugadoresActivos", out int activos);
                return new { Titulo = titulo ?? "", Activos = activos };
            })
            .OrderByDescending(j => j.Activos)
            .Take(5)
            .Select(j => j.Titulo)
            .ToList();

        // 2. Géneros con más torneos activos
        QuerySnapshot torneosActivos = await _firestoreDb.Collection("torneos")
            .WhereEqualTo("estado", "en progreso")
            .GetSnapshotAsync();

        // Mapear torneoId -> juegoId para obtener el género
        var torneoJuegoIds = torneosActivos.Documents
            .Where(d => d.Exists)
            .Select(d => d.ConvertTo<Torneo>().Juego)
            .Distinct()
            .ToList();

        var generoConteo = new Dictionary<string, int>();
        foreach (var juegoId in torneoJuegoIds)
        {
            DocumentSnapshot juegoDoc = await _firestoreDb.Collection("juegos").Document(juegoId).GetSnapshotAsync();
            if (juegoDoc.Exists && juegoDoc.TryGetValue("genero", out string genero) && !string.IsNullOrEmpty(genero))
            {
                if (!generoConteo.ContainsKey(genero)) generoConteo[genero] = 0;
                generoConteo[genero]++;
            }
        }

        var generosMasTorneos = generoConteo
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();

        // 3. Hora pico de actividad (basado en ultimaConexion de jugadores)
        QuerySnapshot jugSnapshot = await _firestoreDb.Collection("jugadores").GetSnapshotAsync();
        var horaConteo = new Dictionary<int, int>();
        foreach (var doc in jugSnapshot.Documents)
        {
            if (!doc.Exists) continue;
            if (doc.TryGetValue("ultimaConexion", out Timestamp ultima))
            {
                int hora = ultima.ToDateTime().ToLocalTime().Hour;
                if (!horaConteo.ContainsKey(hora)) horaConteo[hora] = 0;
                horaConteo[hora]++;
            }
        }

        string horaPico = "Sin datos";
        if (horaConteo.Any())
        {
            int horaMasFrecuente = horaConteo.OrderByDescending(kv => kv.Value).First().Key;
            horaPico = $"{horaMasFrecuente:D2}:00 - {(horaMasFrecuente + 1) % 24:D2}:00";
        }

        TendenciasResponse tendencias = new()
        {
            JuegosMasPopulares = top5Juegos,
            GenerosMasTorneos = generosMasTorneos,
            HoraPicoActividad = horaPico
        };

        return ApiResponse<TendenciasResponse>.Success(tendencias);
    }
}
