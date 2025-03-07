using Microsoft.AspNetCore.Authorization;

namespace Plutus.API.Asp;
public class PlutusUserHandler : AuthorizationHandler<PlutusUserRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PlutusUserRequirement requirement)
    {
        var resourceAccess = context.User.FindFirst("resource_access")?.Value;

        if (resourceAccess != null)
        {
            var resourceAccessJson = System.Text.Json.JsonDocument.Parse(resourceAccess);
            var roles = resourceAccessJson.RootElement
                .GetProperty("plutus-local")
                .GetProperty("roles");

            if (roles.EnumerateArray().Any(role => role.GetString() == "plutus-user"))
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}


public class PlutusUserRequirement : IAuthorizationRequirement
{
    public PlutusUserRequirement() { }
}


