# 01 — Estructura del Proyecto

Este documento describe en detalle la organización de carpetas y archivos del backend.

---

## Árbol de Carpetas

```
examen/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── JugadoresController.cs
│   ├── JuegosController.cs
│   ├── TorneosController.cs
│   ├── ParticipacionesController.cs
│   ├── ClasificacionesController.cs
│   └── ReportesController.cs
│
├── Models/
│   ├── Entities/
│   │   ├── Jugador.cs
│   │   ├── Juego.cs
│   │   ├── Torneo.cs
│   │   ├── Participacion.cs
│   │   ├── EstadisticasParticipacion.cs
│   │   └── Clasificacion.cs
│   │
│   ├── Requests/
│   │   ├── RegistroRequest.cs
│   │   ├── LoginRequest.cs
│   │   ├── ActualizarPerfilJugadorRequest.cs
│   │   ├── CrearJuegoRequest.cs
│   │   ├── ActualizarJuegoRequest.cs
│   │   ├── CrearTorneoRequest.cs
│   │   ├── ActualizarTorneoRequest.cs
│   │   ├── CambiarEstadoTorneoRequest.cs
│   │   └── InscribirseTorneoRequest.cs
│   │
│   └── Responses/
│       ├── ApiResponse.cs
│       ├── AuthResponse.cs
│       ├── JugadorPublicoResponse.cs
│       ├── JuegoResponse.cs
│       ├── TorneoResponse.cs
│       ├── ParticipacionResponse.cs
│       ├── MisTorneosResponse.cs
│       ├── ClasificacionJuegoResponse.cs
│       ├── MiClasificacionResponse.cs
│       ├── MiDesempenoResponse.cs
│       ├── TorneoPopularResponse.cs
│       ├── JugadorDestacadoResponse.cs
│       └── TendenciasResponse.cs
│
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IJugadorService.cs
│   │   ├── IJuegoService.cs
│   │   ├── ITorneoService.cs
│   │   ├── IParticipacionService.cs
│   │   ├── IClasificacionService.cs
│   │   └── IReporteService.cs
│   │
│   ├── AuthService.cs
│   ├── JugadorService.cs
│   ├── JuegoService.cs
│   ├── TorneoService.cs
│   ├── ParticipacionService.cs
│   ├── ClasificacionService.cs
│   └── ReporteService.cs
│
├── Config/
│   ├── FirebaseSettings.cs
│   └── JwtSettings.cs
│
├── Middleware/
│   └── ExceptionMiddleware.cs
│
├── Program.cs
├── appsettings.json
├── .gitignore
└── firebase-credentials.json   ← NO subir al repositorio
```

---

## Descripción de Carpetas

### `Controllers/`
Contiene los controladores HTTP de la API. Cada controlador define las rutas (`[HttpGet]`, `[HttpPost]`, etc.) y se encarga de recibir las solicitudes, validar el `ModelState` y delegar la lógica al servicio correspondiente. No contienen lógica de negocio directa.

### `Models/Entities/`
Clases que representan los documentos almacenados en Firebase Firestore. Cada campo está decorado con el atributo `[FirestoreProperty("nombreExacto")]` para garantizar la correspondencia con los nombres de campo definidos en el examen.

### `Models/Requests/`
DTOs (Data Transfer Objects) de entrada. Define qué datos puede enviar el cliente y aplica las validaciones mediante `DataAnnotations` (`[Required]`, `[StringLength]`, atributos personalizados). Estos objetos nunca se persisten directamente.

### `Models/Responses/`
DTOs de salida. Define exactamente qué información se retorna al cliente, evitando exponer campos sensibles como contraseñas. La clase genérica `ApiResponse<T>` estandariza todas las respuestas.

### `Services/Interfaces/`
Contratos de interfaz para cada servicio. Permite la inyección de dependencias (DI) y facilita futuras pruebas unitarias desacopladas de la implementación concreta.

### `Services/`
Implementaciones de la lógica de negocio. Aquí se ejecutan las consultas a Firestore, las validaciones cruzadas entre colecciones, la generación del JWT y todo el procesamiento de datos.

### `Config/`
Clases POCO (Plain Old C# Object) que mapean las secciones de configuración del archivo `appsettings.json`. Se utilizan con el patrón `IOptions<T>` para inyectar configuraciones de manera tipada.

### `Middleware/`
Contiene el `ExceptionMiddleware`, que intercepta globalmente cualquier excepción no controlada y devuelve una respuesta JSON estandarizada, evitando que el servidor exponga mensajes de error internos.
