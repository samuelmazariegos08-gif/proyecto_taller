using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TallerBackend.Models;

namespace TallerBackend.Services;

public class JwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public (string Token, DateTime ExpiraEn) GenerarToken(Usuario usuario)
    {
        var jwtSection = _configuration.GetSection("Jwt");
        var key = jwtSection["Key"] ?? throw new InvalidOperationException("Falta configurar Jwt:Key.");
        var issuer = jwtSection["Issuer"];
        var audience = jwtSection["Audience"];
        var expirationMinutes = jwtSection.GetValue<int>("ExpirationMinutes", 120);
        var expiraEn = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.IdUsuario.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, usuario.Username),
            new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new(ClaimTypes.Name, usuario.Username),
            new(ClaimTypes.Email, usuario.Correo),
            new(ClaimTypes.Role, usuario.Rol?.Nombre ?? string.Empty)
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiraEn,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEn);
    }
}
