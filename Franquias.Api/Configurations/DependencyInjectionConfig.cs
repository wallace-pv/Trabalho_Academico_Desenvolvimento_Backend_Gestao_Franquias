using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Franquias.Api.Data;
using Franquias.Api.Models;
using Franquias.Api.Repositories.Implementations;
using Franquias.Api.Repositories.Interfaces;
using Franquias.Api.Services.Implementations;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Configurations;

public static class DependencyInjectionConfig
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Contexto EF Core SQLite
        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Franquias.db";
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(connectionString));

        // 2. Repositórios
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnidadeRepository, UnidadeRepository>();
        services.AddScoped<IProdutoRepository, ProdutoRepository>();
        services.AddScoped<IEstoqueRepository, EstoqueRepository>();
        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IRoyaltyRepository, RoyaltyRepository>();
        services.AddScoped<IChamadoRepository, ChamadoRepository>();

        // 3. Serviços de Regra de Negócio
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUnidadeService, UnidadeService>();
        services.AddScoped<IProdutoService, ProdutoService>();
        services.AddScoped<IEstoqueService, EstoqueService>();
        services.AddScoped<IVendaService, VendaService>();
        services.AddScoped<IRoyaltyService, RoyaltyService>();
        services.AddScoped<IFornecedorService, FornecedorService>();
        services.AddScoped<IChamadoService, ChamadoService>();
        services.AddScoped<IRelatorioService, RelatorioService>();

        // 4. Autenticação JWT Bearer
        var jwtKey = configuration["Jwt:Key"] ?? "ChaveSuperSecretaParaAssinaturaDoTokenJwtUninter2026!@#";
        var key = Encoding.UTF8.GetBytes(jwtKey);

        services.AddAuthentication(options =>
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
                ValidIssuer = configuration["Jwt:Issuer"] ?? "FranquiasApi",
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"] ?? "FranquiasApp",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // 5. Configuração do Swagger com Autenticação JWT
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Sistema de Gestão de Franquias - API REST",
                Version = "v1",
                Description = "API REST corporativa para gerenciamento de rede de franquias desenvolvida para o Trabalho Acadêmico UNINTER.\n\n" +
                              "**Aluno:** WALLACE F G SILVA | **RU:** 5146520\n" +
                              "**Orientador:** Prof. Rodrigo da S. do Nascimento\n\n" +
                              "**Perfis de Acesso:**\n" +
                              "- `AdminFranqueadora`: Acesso total e cadastros mestres\n" +
                              "- `GestorUnidade`: Gestão da unidade franqueada e funcionários\n" +
                              "- `Operador`: Operações diárias de PDV e estoque",
                Contact = new OpenApiContact
                {
                    Name = "WALLACE F G SILVA - RU 5146520",
                    Email = "admin@franquias.com.br"
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Insira o token JWT no formato: Bearer {seu_token}",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
