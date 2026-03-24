# Plataforma de Juegos y Torneos — Backend API

Proyecto académico desarrollado para el **Examen Final de Programación Web II**.  
API RESTful construida con **.NET 8**, base de datos en la nube con **Firebase Firestore** y autenticación segura mediante **JWT (JSON Web Tokens)**.

---

## Descripción

Este backend expone una API completa para gestionar una plataforma de videojuegos competitivos que incluye:

- Registro e inicio de sesión de jugadores
- Catálogo de videojuegos
- Creación y gestión de torneos
- Inscripción y seguimiento de participaciones
- Reportes analíticos y tablas de clasificación

---

## Tecnologías Utilizadas

| Tecnología | Versión / Uso |
|---|---|
| .NET | 8.0 (ASP.NET Core Web API) |
| Firebase Firestore | Base de datos NoSQL en la nube |
| Firebase Admin SDK | Conexión servidor-a-servidor |
| JWT (JSON Web Tokens) | Autenticación y autorización |
| BCrypt.Net | Hashing seguro de contraseñas |
| Swagger / Swashbuckle | Documentación interactiva de la API |

---

## Estructura General del Proyecto

```
examen/
├── Controllers/          # Controladores HTTP (endpoints)
├── Models/
│   ├── Entities/         # Entidades que se mapean a Firestore
│   ├── Requests/         # DTOs de entrada y validación
│   └── Responses/        # DTOs de salida hacia el cliente
├── Services/
│   ├── Interfaces/       # Contratos (interfaces)
│   └── *.cs              # Implementaciones de los servicios
├── Config/               # Clases de configuración (Firebase, JWT)
├── Middleware/           # Middleware global de manejo de errores
├── Program.cs            # Punto de entrada y configuración DI
├── appsettings.json      # Parámetros de configuración
└── firebase-credentials.json  ⚠️ NO subir al repositorio
```

> Para mayor detalle ver [`docs/01-estructura-del-proyecto.md`](docs/01-estructura-del-proyecto.md)

---

## Cómo Correr el Proyecto Rápidamente

```bash
# 1. Restaurar dependencias
dotnet restore

# 2. Ejecutar el servidor
dotnet run
```

La API estará disponible en `http://localhost:5288`.  
Swagger UI disponible en `http://localhost:5288/swagger`.

> Para configuración completa ver [`docs/03-ejecutar-proyecto.md`](docs/03-ejecutar-proyecto.md)

---

## ⚠️ Nota Importante: firebase-credentials.json

El archivo `firebase-credentials.json` **contiene las credenciales privadas** de acceso a Firebase y **NO debe subirse nunca al repositorio**.

- Ya está incluido en `.gitignore` automáticamente.
- Cada integrante debe obtener este archivo de forma local.
- Ver instrucciones en [`docs/02-configurar-firebase.md`](docs/02-configurar-firebase.md)

---

## División de Responsabilidades

| Integrante | Escenario |
|---|---|
| Integrante 1 | Autenticación y Gestión de Jugadores |
| Integrante 2 | Gestión de Videojuegos |
| Integrante 3 | Creación y Gestión de Torneos |
| Integrante 4 | Participaciones en Torneos |
| Integrante 5 | Reportes y Clasificaciones |

> Detalle completo en [`docs/04-division-responsabilidades.md`](docs/04-division-responsabilidades.md)
