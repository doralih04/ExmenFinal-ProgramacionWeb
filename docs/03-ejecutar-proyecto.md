# 03 — Cómo Ejecutar el Proyecto

---

## Prerequisitos

Antes de ejecutar el proyecto asegúrarse de tener instalado:

| Requisito | Versión mínima |
|---|---|
| .NET SDK | 8.0 |
| Archivo `firebase-credentials.json` | Obligatorio (ver doc 02) |
| Conexión a internet | Para conectar con Firebase |

Verificar la versión de .NET instalada:

```bash
dotnet --version
# Debe mostrar: 8.x.x
```

Si no está instalado, descargarlo desde: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Pasos para Ejecutar

### 1. Restaurar Paquetes NuGet

```bash
dotnet restore
```

Este comando descarga todas las dependencias declaradas en el archivo `.csproj`.

### 2. Ejecutar el Servidor

```bash
dotnet run
```

Si el SDK de .NET no está en el PATH del sistema (común en macOS), usar:

```bash
export PATH="$HOME/.dotnet:$PATH"
dotnet run
```

### 3. Verificar que el Servidor Esté Activo

Cuando el servidor inicie correctamente, verás en la terminal un mensaje similar a:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5288
```

---

## Abrir Swagger UI

Una vez el servidor esté corriendo, abrir el navegador en:

```
http://localhost:5288/swagger
```

Swagger permite probar todos los endpoints de forma interactiva y revisar los modelos de datos de entrada y salida.

---

## Probar la Autenticación JWT

1. Usar el endpoint `POST /api/auth/login` con las credenciales de un jugador registrado.
2. Copiar el valor del campo `token` del JSON de respuesta.
3. En Swagger, hacer clic en el botón **"Authorize"** (ícono de candado 🔒).
4. Ingresar el token en el formato:
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```
5. A partir de ahí, todos los endpoints protegidos con `[Authorize]` estarán disponibles.

---

## Errores Comunes

### ❌ `The file 'firebase-credentials.json' was not found`
**Causa:** El archivo de credenciales de Firebase no existe en la ruta esperada.  
**Solución:** Seguir los pasos del [documento de configuración](02-configurar-firebase.md) y colocarlo en la raíz del proyecto.

### ❌ `Could not load type 'Google.Cloud.Firestore.FirestoreDb'`
**Causa:** Paquetes NuGet no restaurados.  
**Solución:** Ejecutar `dotnet restore` nuevamente.

### ❌ Error 401 Unauthorized en endpoints protegidos
**Causa:** El token JWT ha expirado o no fue enviado en el header.  
**Solución:** Iniciar sesión nuevamente con `POST /api/auth/login` y actualizar el token en Swagger.

### ❌ Error 403 Forbidden en endpoints de admin
**Causa:** El jugador autenticado no tiene el rol requerido.  
**Solución:** Usar una cuenta cuyo campo `rol` en Firestore sea `"admin"` u `"organizador"` según corresponda.
