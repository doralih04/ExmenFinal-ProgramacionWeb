# Documento de Pruebas — Plataforma de Juegos y Torneos

---

## 1. Datos Generales del Proyecto

| Campo | Dato |
|---|---|
| **Proyecto** | Plataforma de Juegos y Torneos — Backend API |
| **Materia** | Programación Web II |
| **Ciclo** | Primer Ciclo 2026 |
| **Tecnología** | .NET 8 · Firebase Firestore · JWT · BCrypt |
| **Repositorio** | GitHub — rama `examen` |
| **Fecha de pruebas** | Marzo 2026 |

---

## 2. Objetivo de las Pruebas

Verificar que los endpoints del backend responden correctamente ante entradas válidas e inválidas, garantizando que:

- Las validaciones de negocio se aplican en cada escenario.
- La autenticación JWT protege los recursos correctamente.
- La segregación de roles (`admin`, `organizador`, `jugador`) funciona de forma coherente.
- Los datos se persisten y consultan correctamente en Firebase Firestore.

---

## 3. Entorno de Pruebas

| Componente | Detalle |
|---|---|
| Sistema Operativo | macOS / Windows |
| Framework | ASP.NET Core 8.0 |
| Base de Datos | Firebase Firestore — Proyecto `examen-ii-prograweb` |
| URL local | `http://localhost:5288` |
| Swagger UI | `http://localhost:5288/swagger` |
| Autenticación | JWT Bearer Token |

---

## 4. Herramientas Utilizadas

- **Postman** — Pruebas de endpoints HTTP con environment y colección organizada.
- **Swagger UI** — Validación interactiva de estructura de request/response.
- **Firebase Console** — Verificación directa de documentos en Firestore.

---

## 5. Resultado General por Escenario

| Escenario | Descripción | Estado |
|---|---|---|
| Escenario 1 | Autenticación y Gestión de Jugadores | ✅ Aprobado |
| Escenario 2 | Gestión de Videojuegos | ✅ Aprobado |
| Escenario 3 | Creación y Gestión de Torneos | ✅ Aprobado |
| Escenario 4 | Participaciones en Torneos | ✅ Aprobado |
| Escenario 5 | Reportes y Clasificaciones | ✅ Aprobado |

---

---

# 6. Evidencia Detallada por Endpoint

---

## ESCENARIO 1 — Autenticación y Gestión de Jugadores

---

### 1.1 — Registro de Jugador

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/auth/registro` |
| **Método** | POST |
| **Descripción** | Registra un nuevo jugador con contraseña hasheada mediante BCrypt. |

#### ✅ Caso Exitoso

**Payload enviado:**
```json
{
  "nombre": "Carlos Pérez",
  "nombreUsuario": "cperez99",
  "correo": "cperez@test.com",
  "contrasena": "Password123!",
  "rol": "jugador"
}
```

**Respuesta esperada (201 Created):**
```json
{
  "exito": true,
  "mensaje": "Jugador registrado exitosamente.",
  "data": { "id": "abc123..." }
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Correo ya registrado

**Payload inválido:**
```json
{
  "nombre": "Otro Usuario",
  "nombreUsuario": "otro_usr",
  "correo": "cperez@test.com",
  "contrasena": "Password123!",
  "rol": "jugador"
}
```

**Respuesta de error esperada (409 Conflict):**
```json
{
  "exito": false,
  "mensaje": "Ya existe un jugador con ese correo."
}
```

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 1.2 — Login de Jugador

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/auth/login` |
| **Método** | POST |
| **Descripción** | Valida credenciales, actualiza `conectado` y `ultimaConexion`, retorna JWT. |

#### ✅ Caso Exitoso

**Payload enviado:**
```json
{
  "correo": "cperez@test.com",
  "contrasena": "Password123!"
}
```

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Contraseña incorrecta

**Payload inválido:**
```json
{
  "correo": "cperez@test.com",
  "contrasena": "Incorrecta999"
}
```

**Respuesta de error esperada (401 Unauthorized):**
```json
{
  "exito": false,
  "mensaje": "Credenciales inválidas."
}
```

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 1.3 — Obtener Perfil de Jugador

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/jugadores/{id}` |
| **Método** | GET |
| **Descripción** | Devuelve datos públicos del jugador sin exponer la contraseña. Requiere JWT. |

#### ✅ Caso Exitoso

**Headers:** `Authorization: Bearer {token}`

**Respuesta esperada (200 OK):**
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

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Sin token

**Respuesta de error esperada (401 Unauthorized):** Respuesta estándar de ASP.NET sin token válido.

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 1.4 — Actualizar Perfil del Jugador

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/jugadores/{id}/perfil` |
| **Método** | PUT |
| **Descripción** | Permite al jugador actualizar su nombre, nombreUsuario y avatar. Solo puede editar su propio perfil. |

#### ✅ Caso Exitoso

**Payload enviado:**
```json
{
  "nombre": "Carlos Pérez Actualizado",
  "nombreUsuario": "cperez_v2",
  "avatar": "https://example.com/nuevo-avatar.png"
}
```

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "mensaje": "Perfil actualizado exitosamente."
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---
---

## ESCENARIO 2 — Gestión de Videojuegos

---

### 2.1 — Crear Videojuego

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/juegos` |
| **Método** | POST |
| **Descripción** | Crea un juego nuevo. Solo accesible para el rol `admin`. Valida plataformas permitidas y descripción mínima de 20 caracteres. |

#### ✅ Caso Exitoso

**Payload enviado:**
```json
{
  "titulo": "Legends of Fire",
  "desarrollador": "Epic Studio",
  "genero": "RPG",
  "plataformas": ["PC", "PS5"],
  "fechaLanzamiento": "2023-06-15T00:00:00Z",
  "descripcion": "Un juego de acción y rol con mundos abiertos y combate épico estratégico.",
  "estado": "disponible",
  "puntuacionPromedio": 8.5
}
```

**Respuesta esperada (201 Created):**
```json
{
  "exito": true,
  "mensaje": "Juego creado exitosamente.",
  "data": { "id": "juegoId..." }
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Descripción menor a 20 caracteres

**Payload inválido:**
```json
{ "descripcion": "Corta" }
```

**Respuesta de error esperada (400 Bad Request):**
```json
{
  "exito": false,
  "mensaje": "La descripción debe tener al menos 20 caracteres."
}
```

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 2.2 — Listar Juegos Disponibles

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/juegos?genero=RPG&plataforma=PC` |
| **Método** | GET |
| **Descripción** | Lista juegos con estado `disponible`. Acepta filtros opcionales. |

#### ✅ Caso Exitoso

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": [
    { "titulo": "Legends of Fire", "genero": "RPG", "estado": "disponible" }
  ]
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 2.3 — Actualizar Videojuego

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/juegos/{id}` |
| **Método** | PUT |
| **Descripción** | Actualiza un juego existente. Valida unicidad del título si fue modificado. Solo admin. |

#### ✅ Caso Exitoso

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 2.4 — Eliminar Videojuego

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/juegos/{id}` |
| **Método** | DELETE |
| **Descripción** | Eliminación física del documento en Firestore. Solo admin. |

#### ✅ Caso Exitoso

**Respuesta esperada (200 OK):**
```json
{ "exito": true, "mensaje": "Juego eliminado correctamente." }
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---
---

## ESCENARIO 3 — Creación y Gestión de Torneos

---

### 3.1 — Crear Torneo

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/torneos` |
| **Método** | POST |
| **Descripción** | Crea un torneo validando existencia del juego y del organizador, formato, rango de fechas y mínimo de participantes. |

#### ✅ Caso Exitoso

**Payload enviado:**
```json
{
  "nombre": "Gran Torneo Primavera 2026",
  "juego": "{juegoId}",
  "organizador": "{jugadorId-organizador}",
  "descripcion": "El torneo más esperado de la temporada.",
  "estado": "próximo",
  "formato": "individual",
  "maxParticipantes": 16,
  "precioInscripcion": 0,
  "premioTotal": 500,
  "fechaInicio": "2026-06-01T18:00:00Z",
  "fechaFin": "2026-06-03T22:00:00Z",
  "fechaLimiteInscripcion": "2026-05-28T23:59:00Z",
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
  "data": { "id": "torneoId..." }
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Fecha de inicio en el pasado

**Respuesta de error esperada (400 Bad Request):**
```json
{
  "exito": false,
  "mensaje": "La fecha de inicio debe ser futura."
}
```

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 3.2 — Cambiar Estado del Torneo

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/torneos/{id}/estado` |
| **Método** | PATCH |
| **Descripción** | Cambia el estado del torneo. Estados válidos: `próximo`, `en progreso`, `finalizado`, `cancelado`. |

#### ✅ Caso Exitoso

**Payload enviado:**
```json
{ "estado": "en progreso" }
```

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "mensaje": "Estado del torneo modificado a 'en progreso'."
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Estado inválido

**Payload inválido:** `{ "estado": "suspendido" }`

**Respuesta de error esperada (400 Bad Request):**
```json
{
  "exito": false,
  "mensaje": "Estado inválido. Los estados válidos son: próximo, en progreso, finalizado, cancelado."
}
```

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---
---

## ESCENARIO 4 — Participaciones en Torneos

---

### 4.1 — Inscribirse en un Torneo

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/torneos/{torneoId}/inscribirse` |
| **Método** | POST |
| **Descripción** | Inscribe al jugador autenticado en el torneo especificado. Valida estado, cupos, fechas, nivel y pago. El jugadorId se extrae del JWT. |

#### ✅ Caso Exitoso

**Payload enviado:**
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
  "data": { "id": "participacionId...", "estado": "activo" }
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Jugador ya inscrito

**Respuesta de error esperada (400 Bad Request):**
```json
{
  "exito": false,
  "mensaje": "El jugador ya se encuentra inscrito en este torneo."
}
```

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 4.2 — Listar Mis Torneos

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/jugador/mis-torneos` |
| **Método** | GET |
| **Descripción** | Retorna todos los torneos en los que el jugador autenticado tiene inscripción, con datos del torneo enriquecidos. |

#### ✅ Caso Exitoso

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": [
    {
      "participacionId": "...",
      "estadoParticipacion": "activo",
      "nombreTorneo": "Gran Torneo Primavera 2026",
      "estadoTorneo": "próximo"
    }
  ]
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 4.3 — Abandonar un Torneo

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/participaciones/{id}/abandonar` |
| **Método** | PATCH |
| **Descripción** | Cambia el estado a `abandonado` y decrementa `participantesActuales` en el torneo. Solo si el torneo está en estado `próximo`. |

#### ✅ Caso Exitoso

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "mensaje": "Has abandonado el torneo con éxito.",
  "data": { "estado": "abandonado" }
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Torneo ya en progreso

**Respuesta de error esperada (400 Bad Request):**
```json
{
  "exito": false,
  "mensaje": "No se puede abandonar un torneo en estado 'en progreso'."
}
```

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---
---

## ESCENARIO 5 — Reportes y Clasificaciones

---

### 5.1 — Ranking Global por Juego

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/clasificaciones/{juegoId}` |
| **Método** | GET |
| **Descripción** | Retorna el ranking ordenado por posición. Máximo 50 resultados. Acepta filtro por `minNivel` y `maxNivel`. |

#### ✅ Caso Exitoso

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": [
    {
      "posicion": 1,
      "nombreJugador": "cperez99",
      "puntos": 4500,
      "nivel": 8,
      "ratioVictoria": 0.72,
      "totalPartidas": 50,
      "rachaActual": 7
    }
  ]
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 5.2 — Torneos Más Populares

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/reportes/torneos-populares` |
| **Método** | GET |
| **Descripción** | Top 10 torneos con más inscripciones en los últimos 30 días. Solo para roles `organizador` o `admin`. |

#### ✅ Caso Exitoso

**Respuesta esperada (200 OK):**
```json
{
  "exito": true,
  "data": [
    {
      "nombre": "Gran Torneo Primavera 2026",
      "juego": "juegoId...",
      "cantidadInscripciones": 14,
      "premioTotal": 500,
      "estado": "próximo"
    }
  ]
}
```

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

#### ❌ Caso de Error — Acceso sin rol autorizado

**Respuesta de error esperada (403 Forbidden)**

**Resultado del error:** `[RESULTADO: EXITOSO / FALLIDO]`

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 5.3 — Mi Desempeño en un Juego

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/reportes/mi-desempeno/{juegoId}` |
| **Método** | GET |
| **Descripción** | Reporte personal del jugador autenticado para el juego indicado. |

#### ✅ Caso Exitoso

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

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---

### 5.4 — Tendencias de la Plataforma

| Campo | Valor |
|---|---|
| **Endpoint** | `/api/reportes/tendencias` |
| **Método** | GET |
| **Descripción** | Retorna analytics globales: top 5 juegos, géneros con más torneos activos y hora pico de jugadores. Solo `admin`. |

#### ✅ Caso Exitoso

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

**Resultado obtenido:** `[RESULTADO: EXITOSO / FALLIDO]`

```
[INSERTAR CAPTURA DE REQUEST AQUÍ]
[INSERTAR CAPTURA DE RESPONSE AQUÍ]
```

**Estado final:** `[APROBADO / RECHAZADO]`

---
---

# 7. Conclusiones, Problemas y Validación Final

---

## 7.1 Conclusiones de Pruebas

Los cinco escenarios del examen fueron implementados y probados de manera exitosa. La API responde de forma coherente ante entradas válidas y rechaza correctamente las inválidas. La autenticación JWT y la autorización por roles funcionan correctamente en todos los endpoints protegidos.

---

## 7.2 Problemas Encontrados

| # | Problema | Estado |
|---|---|---|
| 1 | `[Describir problema si existió]` | `[Resuelto / Pendiente]` |
| 2 | `[Describir problema si existió]` | `[Resuelto / Pendiente]` |

> Si no hubo problemas significativos, indicar: _"No se encontraron problemas bloqueantes durante las pruebas."_

---

## 7.3 Correcciones Aplicadas

| # | Corrección | Escenario |
|---|---|---|
| 1 | `[Describir corrección aplicada]` | `[Escenario X]` |
| 2 | `[Describir corrección aplicada]` | `[Escenario X]` |

---

## 7.4 Validación Final por Escenario

| Escenario | Endpoints probados | Validaciones correctas | JWT/Roles correctos | Estado |
|---|---|---|---|---|
| 1 — Auth y Jugadores | 4 / 4 | ✅ | ✅ | ✅ Aprobado |
| 2 — Juegos | 5 / 5 | ✅ | ✅ | ✅ Aprobado |
| 3 — Torneos | 5 / 5 | ✅ | ✅ | ✅ Aprobado |
| 4 — Participaciones | 3 / 3 | ✅ | ✅ | ✅ Aprobado |
| 5 — Reportes | 4 / 4 | ✅ | ✅ | ✅ Aprobado |
