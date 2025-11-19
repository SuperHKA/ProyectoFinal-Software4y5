using API_BancoFisclase.Servicios;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register CORS policy (adjust origins for your clients)
var AllowLocalhostOrigins = "_AllowLocalhostOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: AllowLocalhostOrigins, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5202",
                "https://localhost:5202",
                "http://localhost:5000",
                "https://localhost:5001"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
            // .AllowCredentials(); // enable if you need cookies/credentials
    });
});

// Register our Banco service (uses IConfiguration to read connection string)
// Note: register the actual implementation type `APIServicioBanco`.
builder.Services.AddSingleton<IServicioBanco, APIServicioBanco>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Global exception handling middleware (runs for all requests)
// - In Development keep DeveloperExceptionPage for full stack traces.
// - In non-Development use a safe JSON response and map some common exceptions.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
}
else
{
    // Keep a lightweight, explicit middleware for mapping exceptions to status codes
    app.Use(async (context, next) =>
    {
        try
        {
            await next().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Log the exception
            app.Logger.LogError(ex, "Unhandled exception");

            // Map common exception types to HTTP status codes
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

            // Do not expose sensitive details in production
            var payload = JsonSerializer.Serialize(new
            {
                error = statusCode == StatusCodes.Status500InternalServerError
                    ? "An unexpected error occurred."
                    : ex.Message
            });

            await context.Response.WriteAsync(payload).ConfigureAwait(false);
        }
    });
    // Optional: app.UseHsts();
}

app.UseHttpsRedirection();

// Enable CORS (must be called before authentication/authorization and before MapControllers)
app.UseCors(AllowLocalhostOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
