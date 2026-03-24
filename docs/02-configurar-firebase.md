# 02 — Configuración de Firebase

Este documento explica paso a paso cómo configurar Firebase Firestore y las credenciales de cuenta de servicio para que el backend pueda conectarse a la base de datos.

---

## 1. Crear el Proyecto en Firebase

1. Ir a [https://console.firebase.google.com/](https://console.firebase.google.com/)
2. Hacer clic en **"Agregar proyecto"** o seleccionar el proyecto existente del equipo.
3. Seguir el asistente de creación (el Google Analytics es opcional, puede deshabilitarse).

---

## 2. Habilitar Firestore en Modo Prueba

1. En el menú lateral izquierdo, ir a **Firestore Database**.
2. Hacer clic en **"Crear base de datos"**.
3. Seleccionar **"Comenzar en modo de prueba"**.
4. Elegir la ubicación del servidor más cercana (ej. `us-central1`).
5. Hacer clic en **"Listo"**.

> El modo de prueba permite lectura/escritura libre durante 30 días, suficiente para el examen.

---

## 3. Generar las Credenciales de Cuenta de Servicio

Estas credenciales permiten que el servidor .NET se comunique con Firestore de forma segura, sin necesidad de login de usuario.

1. Ir al ícono de **⚙️ (Configuración del proyecto)** en la esquina superior izquierda.
2. Seleccionar **"Configuración del proyecto"**.
3. Hacer clic en la pestaña **"Cuentas de servicio"** (Service accounts).
4. Hacer clic en el botón azul **"Generar nueva clave privada"**.
5. Confirmar la descarga en el diálogo que aparece.
6. Se descargará un archivo `.json` con el formato: `nombre-proyecto-firebase-adminsdk-xxxxx.json`

---

## 4. Colocar el Archivo en el Proyecto

1. Renombrar el archivo descargado a exactamente: **`firebase-credentials.json`**
2. Moverlo a la **carpeta raíz del proyecto**, al mismo nivel que `Program.cs` y `appsettings.json`:

```
examen/
├── firebase-credentials.json   ← aquí
├── Program.cs
├── appsettings.json
└── ...
```

> ⚠️ **Importante:** Este archivo ya está incluido en `.gitignore`. Nunca debe subirse al repositorio de Git bajo ninguna circunstancia, ya que contiene claves privadas de acceso a la base de datos.

---

## 5. Configurar `appsettings.json`

Abrir el archivo `appsettings.json` y asegurarse de que la sección `FirebaseSettings` tenga el ID correcto del proyecto:

```json
"FirebaseSettings": {
  "ProjectId": "examen-ii-prograweb",
  "CredentialsPath": "firebase-credentials.json"
},
```

> El `ProjectId` se encuentra en: Configuración del proyecto → pestaña General → campo **"ID del proyecto"**.

---

## 6. Configuración de JWT

La sección `JwtSettings` en `appsettings.json` controla la generación de tokens de autenticación:

```json
"JwtSettings": {
  "SecretKey": "EstaEsUnaLLaveRequetemuySecretaParaJuegosTorneos12345!!",
  "Issuer": "JuegosTorneosApi",
  "Audience": "JuegosTorneosClients",
  "ExpiryMinutes": 120
}
```

- **SecretKey**: Clave con la que se firma el token. Debe tener al menos 32 caracteres.
- **ExpiryMinutes**: El token expira después de 120 minutos (2 horas).

> En un entorno de producción real, la `SecretKey` debería guardarse en variables de entorno, no en el archivo de configuración.
