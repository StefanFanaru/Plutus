using Microsoft.OpenApi.Models;

namespace Plutus.API.Asp;

public static class SwaggerConfiguration
{
    public static void AddSwaggerServices(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Plutus.Infrastructure", Version = "v1" });
            c.CustomSchemaIds(x => x.FullName.Replace($"{x.Namespace}.", "").Replace("+", ""));


            c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri("https://auth.stefanaru.com/realms/stefanaru/protocol/openid-connect/auth"),
                        Scopes = new Dictionary<string, string>()
                    }
                }
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "oauth2"
                        }
                    },
                    []
                }
            });
        });
    }
}
