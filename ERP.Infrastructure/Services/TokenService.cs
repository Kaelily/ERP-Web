using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ERP.Application.Interfaces;
using ERP.Domain.Entities.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ERP.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public (string token, DateTime expiration) GenerateAccessToken(Usuario usuario)
    {
        var secret = _config["Jwt:Secret"] ?? "ERP_Web_Secret_Key_Super_Secure_2026_DotNet10_BlazorWasm!";
        var issuer = _config["Jwt:Issuer"] ?? "ERP.API";
        var audience = _config["Jwt:Audience"] ?? "ERP.Client";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiration = DateTime.UtcNow.AddMinutes(15);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Name, usuario.Nome),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Role, usuario.Perfil?.Nome ?? "Usuario"),
            new("PerfilId", usuario.PerfilId.ToString())
        };

        if (usuario.VendedorId.HasValue)
        {
            claims.Add(new Claim("VendedorId", usuario.VendedorId.Value.ToString()));
        }

        if (usuario.Perfil?.Modulos != null)
        {
            foreach (var m in usuario.Perfil.Modulos)
            {
                claims.Add(new Claim($"Modulo_{m.Modulo}", m.NivelAcesso.ToString()));
                if (m.PodeLer) claims.Add(new Claim("Permissao", $"{m.Modulo}:Ler"));
                if (m.PodeCriar) claims.Add(new Claim("Permissao", $"{m.Modulo}:Criar"));
                if (m.PodeEditar) claims.Add(new Claim("Permissao", $"{m.Modulo}:Editar"));
                if (m.PodeExcluir) claims.Add(new Claim("Permissao", $"{m.Modulo}:Excluir"));
            }
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return (tokenHandler.WriteToken(token), expiration);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
