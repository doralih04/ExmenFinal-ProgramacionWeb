# 04 — División de Responsabilidades

Este documento detalla la distribución del trabajo entre los 5 integrantes del equipo, siguiendo los escenarios definidos en el examen.

---

## Resumen General

| Integrante | Escenario | Colección Firestore Principal |
|---|---|---|
| Integrante 1 | Autenticación y Gestión de Jugadores | `jugadores` |
| Integrante 2 | Gestión de Videojuegos | `juegos` |
| Integrante 3 | Creación y Gestión de Torneos | `torneos` |
| Integrante 4 | Participaciones en Torneos | `participaciones` |
| Integrante 5 | Reportes y Clasificaciones | `clasificaciones` |

---

## Integrante 1 — Escenario 1: Autenticación y Gestión de Jugadores

### Endpoints Bajo Su Responsabilidad
| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/auth/registro` | Registrar nuevo jugador con hash de contraseña |
| POST | `/api/auth/login` | Login con JWT + actualizar `conectado` y `ultimaConexion` |
| GET | `/api/jugadores/{id}` | Ver perfil público de un jugador (sin contraseña) |
| PUT | `/api/jugadores/{id}/perfil` | Actualizar datos del propio perfil |

### Commits Funcionales
- **Commit 1:** Implementar `POST /api/auth/registro` con BCrypt y estructura inicial del proyecto.
- **Commit 2:** Implementar login con JWT, y endpoints de consulta y actualización de perfil.

### Dependencias con Otros Módulos
- Los **Escenarios 3, 4 y 5** dependen de que los jugadores existan en Firestore para validaciones cruzadas.
- El campo `rol` del jugador es utilizado por todos los módulos para autorización.

---

## Integrante 2 — Escenario 2: Gestión de Videojuegos

### Endpoints Bajo Su Responsabilidad
| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/juegos` | Crear nuevo videojuego (solo admin) |
| GET | `/api/juegos` | Listar juegos disponibles con filtros opcionales |
| GET | `/api/juegos/{id}` | Obtener detalle de un juego |
| PUT | `/api/juegos/{id}` | Actualizar información del juego (solo admin) |
| DELETE | `/api/juegos/{id}` | Eliminar un juego (solo admin) |

### Commits Funcionales
- **Commit 1:** Implementar `POST /api/juegos` con validaciones de plataformas únicas y descripción mínima.
- **Commit 2:** Implementar `GET`, `PUT` y `DELETE` con filtros dinámicos y validación de título único.

### Dependencias con Otros Módulos
- El **Escenario 3** valida que el juego asociado al torneo exista en la colección `juegos`.
- El **Escenario 5** consulta `juegos` para generar el reporte de tendencias.

---

## Integrante 3 — Escenario 3: Creación y Gestión de Torneos

### Endpoints Bajo Su Responsabilidad
| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/torneos` | Crear torneo con validaciones cruzadas |
| GET | `/api/torneos` | Listar torneos con filtros por juego, estado, formato |
| GET | `/api/torneos/{id}` | Obtener detalle de un torneo |
| PUT | `/api/torneos/{id}` | Actualizar torneo respetando límites de participantes |
| PATCH | `/api/torneos/{id}/estado` | Cambiar estado del torneo de forma controlada |

### Commits Funcionales
- **Commit 1:** Implementar `POST /api/torneos` con validaciones de fechas, formato y roles de organizador.
- **Commit 2:** Implementar `GET`, `PUT` y `PATCH /estado` con estados controlados.

### Dependencias con Otros Módulos
- Requiere que el **Escenario 1** haya creado jugadores con rol `organizador` o `admin`.
- Requiere que el **Escenario 2** haya registrado el juego referenciado.
- El **Escenario 4** realiza escrituras en `torneos` para decrementar/incrementar `participantesActuales`.

---

## Integrante 4 — Escenario 4: Participaciones en Torneos

### Endpoints Bajo Su Responsabilidad
| Método | Endpoint | Descripción |
|---|---|---|
| POST | `/api/torneos/{torneoId}/inscribirse` | Inscribir al jugador autenticado en un torneo |
| GET | `/api/jugador/mis-torneos` | Listar todos los torneos del jugador autenticado |
| PATCH | `/api/participaciones/{id}/abandonar` | Abandonar un torneo si aún está en estado "próximo" |

### Commits Funcionales
- **Commit 1:** Implementar `POST /api/torneos/{torneoId}/inscribirse` con todas las validaciones (estado, cupos, nivel, pago).
- **Commit 2:** Implementar `GET /api/jugador/mis-torneos` y `PATCH /api/participaciones/{id}/abandonar`.

### Dependencias con Otros Módulos
- Requiere jugadores del **Escenario 1** y torneos del **Escenario 3**.
- El campo `participantesActuales` en `torneos` es modificado por este módulo.

---

## Integrante 5 — Escenario 5: Reportes y Clasificaciones

### Endpoints Bajo Su Responsabilidad
| Método | Endpoint | Descripción |
|---|---|---|
| GET | `/api/clasificaciones/{juegoId}` | Ranking global por juego (paginado, filtro por nivel) |
| GET | `/api/jugador/clasificacion/{juegoId}` | Posición personal del jugador autenticado |
| GET | `/api/reportes/torneos-populares` | Top 10 torneos más inscritos en 30 días (admin/org) |
| GET | `/api/reportes/jugadores-destacados` | Top 20 jugadores por puntaje global |
| GET | `/api/reportes/mi-desempeno/{juegoId}` | Desempeño personal en un juego específico |
| GET | `/api/reportes/tendencias` | Datos analíticos de la plataforma (solo admin) |

### Commits Funcionales
- **Commit 1:** Implementar endpoints de clasificaciones (`GET /api/clasificaciones/{juegoId}` y `GET /api/jugador/clasificacion/{juegoId}`).
- **Commit 2:** Implementar los 4 endpoints de reportes analíticos con autorización por roles.

### Dependencias con Otros Módulos
- Depende de datos en `jugadores`, `torneos`, `participaciones` y `juegos`.
- Es el módulo de solo lectura más complejo, ya que agrega datos de todas las colecciones.
