using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UserAPI.Models;

namespace UserAPI.Services.Jwt
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public JwtDTO GenerateTokens(User user)
        {
            try
            {
                var issuer = _configuration["JwtSettings:Issuer"];
                var audience = _configuration["JwtSettings:Audience"];
                var key = _configuration["JwtSettings:Key"];
                var tokenValidityMins = _configuration.GetValue<int>("JwtSettings:TokenValidityMins");

                //Adjusting Timezone to be NZ
                var nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                var nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nzTimeZone);
                var tokenExpiryTimeStamp = nzDateTime.AddMinutes(tokenValidityMins);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(JwtRegisteredClaimNames.Name, user.FullName),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email)
                    }),
                    Expires = tokenExpiryTimeStamp,
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature),
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var accessToken = tokenHandler.WriteToken(securityToken);


                var refreshToken = GenerateRefreshToken();
                var refreshTokenExpiry = nzDateTime.AddMinutes(1440);

                return new JwtDTO
                {
                    Id = user.Id,
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    AccessTokenExpiry = tokenExpiryTimeStamp,
                    RefreshTokenExpiry = refreshTokenExpiry,

                };
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public bool ValidateToken(string accessToken)
        {
            var principal = GetTokenPrincipal(accessToken); //for token validation

            //Checks whether is validated or not
            if (!principal.Identity.IsAuthenticated)
            {
                return false;
            }

            return true;
        }

        public ClaimsPrincipal? GetTokenPrincipal(string token)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var validation = new TokenValidationParameters
            {
                ValidIssuer = _configuration["JwtSettings:Issuer"],
                ValidAudience = _configuration["JwtSettings:Audience"],
                IssuerSigningKey = securityKey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,//false because system is expecting the Access Token is already expired, to create a new one.
                ValidateIssuerSigningKey = true
            };

            return new JwtSecurityTokenHandler().ValidateToken(token, validation, out _);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new Byte[64];
            using(var numberGenerator = RandomNumberGenerator.Create())
            {
                numberGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }
    }
}
