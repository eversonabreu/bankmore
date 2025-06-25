using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankMore.Core.Web.JWT;

public class JwtToken
{
    public static string Create(JwtSettings jwtSettings, string accountNumber, string personName)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, accountNumber),
            new Claim(JwtRegisteredClaimNames.UniqueName, personName),
            new Claim(ClaimTypes.NameIdentifier, accountNumber),
            new Claim(ClaimTypes.Name, personName)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}