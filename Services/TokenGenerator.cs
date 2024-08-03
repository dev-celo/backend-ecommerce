using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_ecommerce.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend_ecommerce.Services;
public class TokenGenerator
{
    public string Generate(User user)
    {
        var secret = "kjwfnksdnflkdn9u2r384ru9238448r";
        var expiresDays = 3;

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = AddClaims(user),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = DateTime.Now.AddDays(expiresDays),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    private ClaimsIdentity AddClaims(User user)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim(ClaimTypes.Email, user.Email!));
        claims.AddClaim(new Claim(ClaimTypes.Name, user.UserName!));
        claims.AddClaim(new Claim(ClaimTypes.Role, user.Access!));
        return claims;
    }
}