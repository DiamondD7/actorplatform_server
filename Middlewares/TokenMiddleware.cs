using Microsoft.EntityFrameworkCore;
using UserAPI.Data;
using UserAPI.Services.Jwt;

namespace UserAPI.Middlewares
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        public TokenMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
        {
            string requestPath = context.Request.Path.Value;
            if (requestPath.StartsWith("/api/Users/validate-tokens") || 
                requestPath.StartsWith("/api/Users/logout"))
            {
                await _next(context);
                return;
            }

            var accessToken = context.Request.Cookies["jwt"];
            var refreshToken = context.Request.Cookies["rft"];

            string retrieveAccessToken = "";

            if (string.IsNullOrEmpty(accessToken))
            {
                if(!string.IsNullOrEmpty(refreshToken))
                {
                    retrieveAccessToken = await ProcessTokenGeneration(refreshToken, context, serviceProvider);
                    if (!string.IsNullOrEmpty(retrieveAccessToken))
                    {
                        JwtService tokenService = new JwtService(_config);
                        bool isAuthenticated = tokenService.ValidateToken(retrieveAccessToken);
                        if (!isAuthenticated)
                        {
                            context.Response.StatusCode = 401; //unauthorize
                            return;

                        }
                    }

                    if(context.Response.StatusCode == StatusCodes.Status302Found)
                    {
                        //Complete the response and skip remaining middleware
                        await context.Response.CompleteAsync();
                        return; //Ensure no further processing is done
                    }
                }
            }
            else
            {
                JwtService tokenService = new JwtService(_config);
                bool isAuthenticated = tokenService.ValidateToken(accessToken);

                if (!isAuthenticated)
                {
                    context.Response.StatusCode = 401; //unauthorize
                    return;
                }
            }

            await _next(context);
        }

        private async Task<string> ProcessTokenGeneration(string refreshToken, HttpContext context, IServiceProvider serviceProvider)
        {
            string newAccessToken = "";
            var tokenValidityMins = _config.GetValue<int>("JwtSettings:TokenValidityMins");

            //Adjusting Timezone to be NZ
            var nzTimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
            var nzDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, nzTimeZone);
            var tokenExpiryTimeStamp = nzDateTime.AddMinutes(tokenValidityMins);

            using (var scope = serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userTokenDetails = await dbContext.UsersTable.FirstOrDefaultAsync(x => x.RefreshToken == refreshToken);

                if(userTokenDetails != null)
                {
                    if(nzDateTime > userTokenDetails.RefreshExpiryDate)
                    {
                        context.Response.StatusCode = StatusCodes.Status302Found;
                        return string.Empty;
                    }

                    JwtService jwtService = new JwtService(_config);
                    var newToken = jwtService.GenerateTokens(userTokenDetails);

                    userTokenDetails.RefreshToken = newToken.RefreshToken;
                    userTokenDetails.RefreshExpiryDate = newToken.RefreshTokenExpiry;

                    dbContext.Entry(userTokenDetails).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();


                    context.Response.Cookies.Append("jwt", newToken.AccessToken, new CookieOptions
                    {
                        HttpOnly = true, //Cookies cannot be accessed via JS
                        Secure = true, //Ensures that the cookie is sent over HTTPS
                        SameSite = SameSiteMode.None,
                        Expires = newToken.AccessTokenExpiry,
                    });

                    context.Response.Cookies.Append("rft", newToken.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true, //Cookies cannot be accessed via JS
                        Secure = true, //Ensures that the cookie is sent over HTTPS
                        SameSite = SameSiteMode.None,
                    });


                    newAccessToken = newToken.AccessToken;
                }
            }

            return newAccessToken;
        }

    }
}
