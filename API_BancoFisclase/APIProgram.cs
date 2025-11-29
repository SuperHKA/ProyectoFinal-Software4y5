using API_BancoFisclase.Servicios;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    { 
        Title = "API Banco Fisclase", 
        Version = "v1",
        Description = "API REST para el sistema bancario universitario Fisclase"
    });
});

// Registrar política CORS (ajustar orígenes para tus clientes)
var AllowLocalhostOrigins = "_AllowLocalhostOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowLocalhostOrigins, policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",      // Para el servidor de desarrollo React (EN PRUEBA)
                "http://localhost:5202",
                "http://localhost:5000",
                "https://localhost:5001",
                "https://localhost:7102"

            )
            .AllowAnyHeader()
            .AllowAnyMethod();
            // .AllowCredentials(); // habilitar si necesitas cookies/credenciales
    });
});

// Registrar nuestro servicio Banco (usa IConfiguration para leer la cadena de conexión)
// Nota: registrar el tipo de implementación real `APIServicioBanco`.
builder.Services.AddSingleton<IServicioBanco, APIServicioBanco>();

var app = builder.Build();

// Middleware global de manejo de excepciones (ejecuta para todas las solicitudes)
// - En Desarrollo mantiene DeveloperExceptionPage para stack traces completos.
// - En no-Desarrollo usa una respuesta JSON segura y mapea algunas excepciones comunes.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Habilitar Swagger en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Banco Fisclase v1");
        c.RoutePrefix = string.Empty; // Swagger UI en la raíz: http://localhost:5202/
    });
}
else
{
    // Mantener un middleware liviano y explícito para mapear excepciones a códigos de estado
    app.Use(async (context, next) =>
    {
        try
        {
            await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Registrar la excepción
            app.Logger.LogError(ex, "Excepción no manejada");

            // Mapear tipos comunes de excepción a códigos de estado HTTP
            var statusCode = ex switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                UnauthorizedAccessException => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            // No exponer detalles sensibles en producción
            var payload = JsonSerializer.Serialize(new
            {
                error = statusCode == StatusCodes.Status500InternalServerError
                    ? "Ocurrió un error inesperado."
                    : ex.Message
            });

            await context.Response.WriteAsync(payload).ConfigureAwait(false);
        }
    });
}

// Habilitar redirección HTTPS solo cuando un endpoint HTTPS esté configurado.
// Cuando la aplicación se lanza sin un listener HTTPS (común en algunas ejecuciones dev/debug),
// llamar UseHttpsRedirection produce la advertencia: "Falló al determinar el puerto https para redirección."
// Inspeccionamos URLs configuradas y algunas claves comunes de entorno/config para decidir.
var hasHttpsEndpoint =
    app.Urls.Any(u => u.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
    || !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT"))
    || !string.IsNullOrEmpty(builder.Configuration["Kestrel:Endpoints:Https:Url"]);

if (hasHttpsEndpoint)
{
    app.UseHttpsRedirection();
}
else
{
    app.Logger.LogWarning("Omitiendo redirección HTTPS porque no se detectó un endpoint HTTPS. " +
                         "Si quieres redirección HTTPS habilita una URL HTTPS o configura ASPNETCORE_HTTPS_PORT.");
}

// Habilitar CORS (debe llamarse antes de authentication/authorization y antes de MapControllers)
app.UseCors(AllowLocalhostOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
