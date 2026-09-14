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

// Middleware para garantir que o swagger.json use a especificação OpenAPI 3.0.1
// compatível com a validação por Regex do Swagger UI
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value != null && context.Request.Path.Value.EndsWith("swagger.json", StringComparison.OrdinalIgnoreCase))
    {
        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        await next();

        memoryStream.Seek(0, SeekOrigin.Begin);
        var json = await new StreamReader(memoryStream).ReadToEndAsync();

        // Ajusta a versão OpenAPI 3.0.4 para 3.0.1 para evitar o erro "Unable to render this definition" do Swagger UI
        json = json.Replace("\"openapi\": \"3.0.4\"", "\"openapi\": \"3.0.1\"")
                   .Replace("\"openapi\":\"3.0.4\"", "\"openapi\":\"3.0.1\"");

        var bytes = System.Text.Encoding.UTF8.GetBytes(json);
        context.Response.Body = originalBodyStream;
        context.Response.ContentLength = bytes.Length;
        await context.Response.Body.WriteAsync(bytes);
        return;
    }

    await next();
});

// Habilitar Swagger em todos os ambientes para avaliação acadêmica
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema de Gestão de Franquias v1");
    c.RoutePrefix = "swagger";
    c.DocumentTitle = "UNINTER - Gestão de Franquias API";
});

// Redirecionamento da rota raiz e index.html para o Swagger UI
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapGet("/index.html", () => Results.Redirect("/swagger"));

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
