# 05 — Documento de Pruebas

**Proyecto:** Plataforma de Juegos y Torneos — Backend API  
**Materia:** Programación Web II  
**Fecha:** Marzo 2026  
**Entorno:** Local — `http://localhost:5288`  
**Herramienta:** Swagger UI / Postman

---

## Objetivo de las Pruebas

Verificar que cada endpoint de la API responde correctamente bajo condiciones válidas e inválidas, confirmando el funcionamiento de las validaciones de negocio, la autenticación JWT y la integridad de los datos en Firebase Firestore.

---

## Entorno de Pruebas

| Componente | Detalle |
|---|---|
| Framework | .NET 8 ASP.NET Core |
| Base de datos | Firebase Firestore (modo prueba) |
| Autenticación | JWT Bearer Token |
| URL base | `http://localhost:5288` |
| Documentación interactiva | `http://localhost:5288/swagger` |

---

---

# ESCENARIO 1 — Autenticación y Gestión de Jugadores

---

## Prueba 1.1 — Registro de Jugador

**Endpoint:** `POST /api/auth/registro`

**Payload de ejemplo:**
```json
{
  "nombre": "Carlos Pérez",
  "nombreUsuario": "cperez99",
  "correo": "cperez@test.com",
  "contrasena": "Password123!",
  "rol": "jugador"
}
```

**Respuesta esperada exitosa (201 Created):**
```json
{
  "exito": true,
  "mensaje": "Jugador registrado exitosamente.",
  "data": { ... }
}
```

**Respuesta esperada de error (409 Conflict) — correo duplicado:**
```json
{
  "exito": false,
  "mensaje": "Ya existe un jugador con ese correo."
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 1.2 — Login de Jugador

**Endpoint:** `POST /api/auth/login`

**Payload de ejemplo:**
```json
{
  "correo": "cperez@test.com",
  "contrasena": "Password123!"
}
```

**Respuesta esperada exitosa (200 OK):**
```json
{
  "exito": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Respuesta esperada de error (401) — credenciales incorrectas:**
```json
{
  "exito": false,
  "mensaje": "Credenciales inválidas."
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 1.3 — Ver Perfil de Jugador

**Endpoint:** `GET /api/jugadores/{id}`  
**Headers:** `Authorization: Bearer {token}`

**Respuesta esperada exitosa (200 OK):**
```json
{
  "exito": true,
  "data": {
    "nombre": "Carlos Pérez",
    "nombreUsuario": "cperez99",
    "puntosGlobales": 0,
    "torneosGanados": 0
  }
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

---

# ESCENARIO 2 — Gestión de Videojuegos

---

## Prueba 2.1 — Crear Videojuego

**Endpoint:** `POST /api/juegos`  
**Headers:** `Authorization: Bearer {token-admin}`

**Payload de ejemplo:**
```json
{
  "titulo": "Legends of Fire",
  "desarrollador": "Epic Studio",
  "genero": "RPG",
  "plataformas": ["PC", "PS5"],
  "fechaLanzamiento": "2023-06-15T00:00:00Z",
  "descripcion": "Un juego de rol épico con mundos abiertos y combate estratégico.",
  "estado": "disponible",
  "puntuacionPromedio": 8.5
}
```

**Respuesta esperada exitosa (201 Created):**
```json
{
  "exito": true,
  "mensaje": "Juego creado exitosamente."
}
```

**Respuesta esperada de error (400) — plataforma inválida:**
```json
{
  "exito": false,
  "mensaje": "Plataforma(s) inválida(s). Permitidas: PC, PS5, Xbox, Switch."
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 2.2 — Listar Juegos con Filtros

**Endpoint:** `GET /api/juegos?genero=RPG&plataforma=PC`  
**Headers:** `Authorization: Bearer {token}`

**Respuesta esperada exitosa (200 OK):**
```json
{
  "exito": true,
  "data": [
    {
      "titulo": "Legends of Fire",
      "estado": "disponible",
      ...
    }
  ]
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

---

# ESCENARIO 3 — Creación y Gestión de Torneos

---

## Prueba 3.1 — Crear Torneo

**Endpoint:** `POST /api/torneos`  
**Headers:** `Authorization: Bearer {token-organizador}`

**Payload de ejemplo:**
```json
{
  "nombre": "Gran Torneo Primavera",
  "juego": "{juegoId}",
  "organizador": "{jugadorId-organizador}",
  "descripcion": "El torneo más esperado de la temporada.",
  "estado": "próximo",
  "formato": "individual",
  "maxParticipantes": 16,
  "precioInscripcion": 0,
  "premioTotal": 500,
  "fechaInicio": "2026-05-01T18:00:00Z",
  "fechaFin": "2026-05-03T22:00:00Z",
  "fechaLimiteInscripcion": "2026-04-28T23:59:00Z",
  "minNivel": 0,
  "maxNivel": 0,
  "requiereEquipo": false,
  "tamanioEquipo": 1,
  "reglasModificadas": false
}
```

**Respuesta esperada (201 Created):**
```json
{
  "exito": true,
  "mensaje": "Torneo creado exitosamente.",
  "data": { ... }
}
```

**Respuesta esperada de error — organizador sin rol:** 
```json
{
  "exito": false,
  "mensaje": "El usuario no tiene rol de organizador o admin."
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 3.2 — Cambiar Estado de Torneo

**Endpoint:** `PATCH /api/torneos/{id}/estado`  
**Headers:** `Authorization: Bearer {token-organizador}`

**Payload de ejemplo:**
```json
{
  "estado": "en progreso"
}
```

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "mensaje": "Estado del torneo modificado a 'en progreso'."
}
```

**Respuesta esperada de error — estado inválido:**
```json
{
  "exito": false,
  "mensaje": "Estado inválido. Los estados válidos son: próximo, en progreso, finalizado, cancelado."
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

---

# ESCENARIO 4 — Participaciones en Torneos

---

## Prueba 4.1 — Inscribirse en un Torneo

**Endpoint:** `POST /api/torneos/{torneoId}/inscribirse`  
**Headers:** `Authorization: Bearer {token-jugador}`

**Payload de ejemplo:**
```json
{
  "equipoId": "",
  "pagado": false
}
```

**Respuesta esperada (201 Created):**
```json
{
  "exito": true,
  "mensaje": "Inscripción realizada con éxito.",
  "data": { ... }
}
```

**Respuesta esperada de error — jugador ya inscrito:**
```json
{
  "exito": false,
  "mensaje": "El jugador ya se encuentra inscrito en este torneo."
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 4.2 — Ver Mis Torneos

**Endpoint:** `GET /api/jugador/mis-torneos`  
**Headers:** `Authorization: Bearer {token-jugador}`

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": [
    {
      "participacionId": "...",
      "estadoParticipacion": "activo",
      "nombreTorneo": "Gran Torneo Primavera",
      ...
    }
  ]
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 4.3 — Abandonar un Torneo

**Endpoint:** `PATCH /api/participaciones/{id}/abandonar`  
**Headers:** `Authorization: Bearer {token-jugador}`

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "mensaje": "Has abandonado el torneo con éxito."
}
```

**Respuesta esperada de error — torneo en progreso:**
```json
{
  "exito": false,
  "mensaje": "No se puede abandonar un torneo en estado 'en progreso'."
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

---

# ESCENARIO 5 — Reportes y Clasificaciones

---

## Prueba 5.1 — Ranking Global de un Juego

**Endpoint:** `GET /api/clasificaciones/{juegoId}?minNivel=1&maxNivel=10`  
**Headers:** `Authorization: Bearer {token}`

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": [
    {
      "posicion": 1,
      "nombreJugador": "cperez99",
      "puntos": 4500,
      ...
    }
  ]
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 5.2 — Torneos Populares

**Endpoint:** `GET /api/reportes/torneos-populares`  
**Headers:** `Authorization: Bearer {token-admin}`

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": [
    {
      "nombre": "Gran Torneo Primavera",
      "cantidadInscripciones": 12,
      ...
    }
  ]
}
```

**Respuesta esperada de error — acceso sin rol admin:**  
`403 Forbidden`

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 5.3 — Mi Desempeño en un Juego

**Endpoint:** `GET /api/reportes/mi-desempeno/{juegoId}`  
**Headers:** `Authorization: Bearer {token-jugador}`

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": {
    "nivelActual": 5,
    "posicionRanking": 3,
    "progresoSiguienteNivel": 72.5,
    "ratioVictoria": 0.68,
    "rachaActual": 4,
    "medallasOro": 2,
    "medallasPlata": 1,
    "medallasBronce": 3,
    "mejoresTorneos": ["torneoId1", "torneoId2", "torneoId3"]
  }
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```

---

## Prueba 5.4 — Tendencias de la Plataforma

**Endpoint:** `GET /api/reportes/tendencias`  
**Headers:** `Authorization: Bearer {token-admin}`

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": {
    "juegosMasPopulares": ["Legends of Fire", "..."],
    "generosMasTorneos": ["RPG", "FPS"],
    "horaPicoActividad": "20:00 - 21:00"
  }
}
```

```
[INSERTAR CAPTURA AQUÍ]
[RESULTADO: EXITOSO / FALLIDO]
```
