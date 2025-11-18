using API_BancoFisclase.Servicios;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register our Banco service (uses IConfiguration to read connection string)
// Note: do not assign the result to a local variable; call AddSingleton directly.
// Ensure `ServicioBanco` is defined under `API_BancoFisclase.Servicios` and is public.
builder.Services.AddSingleton<IServicioBanco, ServicioBanco>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
