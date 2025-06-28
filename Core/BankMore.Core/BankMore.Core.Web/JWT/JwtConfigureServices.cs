using BankMore.Core.Web.Filters;
using BankMore.Core.Web.HostedServices;
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
        services.AddHostedService<KafkaConsumerHostedService>();

        services.AddControllers(options =>
        {
            options.Filters.Add<AuthorizationFilter>();
        });
    }
}
