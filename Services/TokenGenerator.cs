using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_ecommerce.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend_ecommerce.Services
{
    public class TokenGenerator
    {
        private readonly string _secret = "kjwfnksdnflkdn9u2r384ru9238448r";
        private readonly int _expiresDays = 3;

        public string Generate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = AddClaims(user),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_secret)),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Expires = DateTime.Now.AddDays(_expiresDays),
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

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Opcional: pode ajustar conforme a necessidade
                    ValidateAudience = false, // Opcional: pode ajustar conforme a necessidade
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero // Elimina o tempo de folga padrão de 5 minutos
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch
            {
                // Token inválido
                return null;
            }
        }
    }
}
