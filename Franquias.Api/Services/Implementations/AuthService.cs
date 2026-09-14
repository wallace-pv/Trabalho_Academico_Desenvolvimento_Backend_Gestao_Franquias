using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Franquias.Api.Data;
using Franquias.Api.DTOs.Auth;
using Franquias.Api.Models;
using Franquias.Api.Services.Interfaces;

namespace Franquias.Api.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Unidade)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(request.Senha, usuario.SenhaHash))
        {
            throw new BadHttpRequestException("Credenciais inválidas. Verifique o e-mail e senha informados.");
        }

        if (!usuario.Ativo)
        {
            throw new BadHttpRequestException("Usuário inativo. Entre em contato com a administração da franquia.");
        }

        usuario.UltimoLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "ChaveSuperSecretaParaAssinaturaDoTokenJwtUninter2026!@#");
        var expiration = DateTime.UtcNow.AddHours(8);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil.ToString()),
            new("Perfil", usuario.Perfil.ToString())
        };

        if (usuario.UnidadeId.HasValue)
        {
            claims.Add(new Claim("UnidadeId", usuario.UnidadeId.Value.ToString()));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = _configuration["Jwt:Issuer"] ?? "FranquiasApi",
            Audience = _configuration["Jwt:Audience"] ?? "FranquiasApp",
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponseDto
        {
            Token = tokenHandler.WriteToken(token),
            Expiracao = expiration,
            Usuario = new UsuarioResponseDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Perfil = usuario.Perfil,
                UnidadeId = usuario.UnidadeId,
                UnidadeNome = usuario.Unidade?.Nome,
                Ativo = usuario.Ativo,
                DataCadastro = usuario.DataCadastro,
                UltimoLogin = usuario.UltimoLogin
            }
        };
    }

    public async Task<UsuarioResponseDto> RegistrarUsuarioAsync(RegisterUsuarioDto request)
    {
        var emailExists = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == request.Email.Trim().ToLower());
        if (emailExists)
        {
            throw new BadHttpRequestException("Já existe um usuário cadastrado com este e-mail.");
        }

        if (request.UnidadeId.HasValue)
        {
            var unidadeExists = await _context.Unidades.AnyAsync(u => u.Id == request.UnidadeId.Value);
            if (!unidadeExists)
            {
                throw new BadHttpRequestException("A unidade informada para o usuário não foi encontrada.");
            }
        }

        var usuario = new Usuario
        {
            Nome = request.Nome.Trim(),
            Email = request.Email.Trim().ToLower(),
            SenhaHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
            Perfil = request.Perfil,
            UnidadeId = request.UnidadeId,
            Ativo = true,
            DataCadastro = DateTime.UtcNow
        };

        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();

        string? unidadeNome = null;
        if (usuario.UnidadeId.HasValue)
        {
            unidadeNome = await _context.Unidades.Where(u => u.Id == usuario.UnidadeId.Value).Select(u => u.Nome).FirstOrDefaultAsync();
        }

        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            UnidadeId = usuario.UnidadeId,
            UnidadeNome = unidadeNome,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro
        };
    }

    public async Task<IEnumerable<UsuarioResponseDto>> ListarUsuariosAsync(int? unidadeId = null, bool? ativo = null)
    {
        var query = _context.Usuarios.Include(u => u.Unidade).AsQueryable();

        if (unidadeId.HasValue)
            query = query.Where(u => u.UnidadeId == unidadeId.Value);

        if (ativo.HasValue)
            query = query.Where(u => u.Ativo == ativo.Value);

        var list = await query.OrderBy(u => u.Nome).ToListAsync();

        return list.Select(u => new UsuarioResponseDto
        {
            Id = u.Id,
            Nome = u.Nome,
            Email = u.Email,
            Perfil = u.Perfil,
            UnidadeId = u.UnidadeId,
            UnidadeNome = u.Unidade?.Nome,
            Ativo = u.Ativo,
            DataCadastro = u.DataCadastro,
            UltimoLogin = u.UltimoLogin
        });
    }

    public async Task<UsuarioResponseDto> AtualizarStatusUsuarioAsync(int id, bool ativo)
    {
        var usuario = await _context.Usuarios.Include(u => u.Unidade).FirstOrDefaultAsync(u => u.Id == id);
        if (usuario == null)
            throw new KeyNotFoundException("Usuário não encontrado.");

        usuario.Ativo = ativo;
        await _context.SaveChangesAsync();

        return new UsuarioResponseDto
        {
            Id = usuario.Id,
            Nome = usuario.Nome,
            Email = usuario.Email,
            Perfil = usuario.Perfil,
            UnidadeId = usuario.UnidadeId,
            UnidadeNome = usuario.Unidade?.Nome,
            Ativo = usuario.Ativo,
            DataCadastro = usuario.DataCadastro,
            UltimoLogin = usuario.UltimoLogin
        };
    }
}
