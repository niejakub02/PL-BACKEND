using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using pl_backend.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace pl_backend.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
        Token? ValidateToken(string authorization);
        User GetCurrentUser();
    }
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenService(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Token:Key")));
            System.Diagnostics.Debug.WriteLine(_configuration.GetValue<string>("Token:Key"));
            
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        public Token? ValidateToken(string authorization)
        {
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                try
                {
                    var parameter = headerValue.Parameter;
                    var jwt = new JwtSecurityTokenHandler();
                    System.Diagnostics.Debug.WriteLine(parameter);
                    var validationParams = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetValue<string>("Token:Key"))),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };

                    jwt.ValidateToken(parameter, validationParams, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;

                    int userId = int.Parse(jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);

                    Token token = new Token
                    {
                        Id = userId
                    };

                    return token;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public User GetCurrentUser()
        {
            var identity = _contextAccessor?.HttpContext?.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                string? nameIdentifier = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                if (nameIdentifier != null)
                {
                    return new User
                    {
                        Id = int.Parse(nameIdentifier)
                    };
                }
            }
            return null;
        }
    }
}
