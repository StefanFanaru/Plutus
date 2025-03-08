
using System.Security.Claims;
using Newtonsoft.Json;

namespace Plutus.API.Asp;
public class UserInfo : IUserInfo
{
    public UserInfo(IHttpContextAccessor httpContextAccessor)
    {
        Id = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(Id))
        {
            throw new Exception("Unable to read user claims");
        }

        Name = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Email)?.Value;
        FirstName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.GivenName)?.Value;
        LastName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Surname)?.Value;
        Email = httpContextAccessor.HttpContext.User.FindFirst("name")?.Value;
        UserName = httpContextAccessor.HttpContext.User.FindFirst("preferred_username")?.Value;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
}
