using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace Plutus.API.Asp;

public class PlutusUserHandler(IConfiguration configuration) : AuthorizationHandler<PlutusUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlutusUserRequirement requirement)
    {
        var resourceAccess = context.User.FindFirst("resource_access")?.Value;

        if (resourceAccess != null)
        {
            var clientId = configuration["AuthClientId"];
            var resourceAccessJson = JsonDocument.Parse(resourceAccess);
            if (!resourceAccessJson.RootElement.TryGetProperty(clientId, out var _))
            {
                return Task.CompletedTask;
            }

            var roles = resourceAccessJson.RootElement
                .GetProperty(clientId)
                .GetProperty("roles");

            if (roles.EnumerateArray().Any(role => role.GetString() == "plutus-user"))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        return Task.CompletedTask;
    }
}

public class PlutusUserRequirement : IAuthorizationRequirement
{
}
