using System.Text;
using Google.Cloud.Firestore;
using JuegosTorneosApi.Config;
using JuegosTorneosApi.Middleware;
using JuegosTorneosApi.Services;
using JuegosTorneosApi.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Configuraciones
builder.Services.Configure<FirebaseSettings>(builder.Configuration.GetSection("FirebaseSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Firebase Environment Variable
var firebaseSettings = builder.Configuration.GetSection("FirebaseSettings").Get<FirebaseSettings>();
if (firebaseSettings != null && !string.IsNullOrEmpty(firebaseSettings.CredentialsPath))
{
    // Solo para el examen: configuramos la variable de entorno, asumiendo que el archivo de credenciales estará en la ruta indicada.
    Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebaseSettings.CredentialsPath);
}

// Inyectar FirestoreDb (Patrón Singleton)
builder.Services.AddSingleton<FirestoreDb>(provider =>
{
    var projectId = firebaseSettings?.ProjectId ?? "tu-proyecto-por-defecto";
    try
    {
        return FirestoreDb.Create(projectId);
    }
    catch (Exception)
    {
        // En caso de que no haya credenciales, devolvemos un mock o ignoramos si falla en tiempo de diseño, pero para runtime lanzará error de auth.
        // Dado que es un examen, dejamos que el error se lance si intentan llamar a BD sin archivo subido. 
        return FirestoreDb.Create(projectId);
    }
});

// Configurar JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings?.SecretKey ?? "ClaveMockSecretaExtensaPorSiFalla12345!!");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings?.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

// Registrar Servicios (DI)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJugadorService, JugadorService>();
builder.Services.AddScoped<IJuegoService, JuegoService>();
builder.Services.AddScoped<ITorneoService, TorneoService>();
builder.Services.AddScoped<IParticipacionService, ParticipacionService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
