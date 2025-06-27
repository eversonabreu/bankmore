using BankMore.Core.Web.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BankMore.Core.Web.JWT;

public static class JwtConfigureServices
{
    public static void ConfigureJwtServices(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

        services.AddMemoryCache();

        services.AddControllers(options =>
        {
            options.Filters.Add<AuthorizationFilter>();
        })
        .ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = new List<object>();

                foreach (var kvp in context.ModelState)
                {
                    foreach (var error in kvp.Value!.Errors)
                    {
                        var parts = error.ErrorMessage.Split('|', 2);

                        if (parts.Length == 2)
                        {
                            errors.Add(new
                            {
                                message = parts[1],
                                code = parts[0]
                            });
                        }
                        else
                        {
                            errors.Add(new
                            {
                                message = error.ErrorMessage
                            });
                        }
                    }
                }

                return new BadRequestObjectResult(new { errors });
            };
        });
    }
}
