using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend_ecommerce.Models;
using Microsoft.IdentityModel.Tokens;

namespace backend_ecommerce.Services
{
    public class TokenGenerator
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly int _expiresDays;

        public TokenGenerator(IConfiguration configuration)
        {
            _secret = configuration["Jwt:Secret"];
            _issuer = configuration["Jwt:Issuer"];
            _expiresDays = int.Parse(configuration["Jwt:ExpiresDays"]);
        }

        public string Generate(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret); // Certifique-se que o _secret é o mesmo em ambos
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = AddClaims(user),
                Expires = DateTime.Now.AddDays(_expiresDays),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                ),
                Issuer = _issuer,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsIdentity AddClaims(User user)
        {
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())); // Adicione o ID do usuário aqui
            claims.AddClaim(new Claim(ClaimTypes.Email, user.Email!));
            claims.AddClaim(new Claim(ClaimTypes.Name, user.UserName!));
            claims.AddClaim(new Claim(ClaimTypes.Role, user.Access!));
            return claims;
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secret); // Mesma chave secreta que no gerador

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key), // Mesma chave secreta
                    ValidateAudience = false, // Também pode desativar se não precisar de audience
                    ValidateLifetime = true, // Validação de expiração
                    ValidateIssuer = true,
                    ClockSkew = TimeSpan.Zero, // Evitar o clock skew de 5 minutos padrão
                    ValidIssuer = _issuer,
                }, out SecurityToken validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                // Adicione um log para depuração se necessário
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
