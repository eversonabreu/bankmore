using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace BankMore.Core.Web.Swagger;

public static class SwaggerConfiguration
{
    public static void AddSwaggerConfiguration(this IServiceCollection services, string apiName, string version)
    {
        services.AddSwaggerGen(swg =>
        {
            swg.SwaggerDoc(version, new OpenApiInfo { Title = apiName, Version = version });

            swg.AddSecurityDefinition("authorization", new OpenApiSecurityScheme
            {
                Name = "authorization",
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header
            });

            swg.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                          Reference = new OpenApiReference
                          {
                            Type = ReferenceType.SecurityScheme,
                            Id = "authorization"
                          }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }

    public static void UseSwaggerConfiguration(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}