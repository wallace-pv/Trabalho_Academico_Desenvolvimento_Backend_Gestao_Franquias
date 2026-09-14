using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Franquias.Api.Configurations;
using Franquias.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuração dos Controllers com serialização limpa de Enums para String
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Injeção de dependência dos módulos da aplicação, EF Core, JWT e Swagger
builder.Services.AddApplicationServices(builder.Configuration);

// Configuração de CORS para permitir consumo de interfaces frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Inicialização e Seed do Banco de Dados Relacional
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await DbInitializer.SeedAsync(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao executar o Seed do banco de dados.");
    }
}

// Middleware global para tratamento padronizado de exceções e retornos HTTP coerentes
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Habilitar Swagger em todos os ambientes para avaliação acadêmica
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Gestão de Franquias v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz da aplicação (http://localhost:5000/)
    c.DocumentTitle = "UNINTER - Gestão de Franquias API";
});

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
