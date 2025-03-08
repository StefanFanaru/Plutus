using Microsoft.AspNetCore.Authorization;

namespace Plutus.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MiscController(IServiceProvider serviceProvider) : ControllerBase
{

    [HttpGet("get-data")]
    public async Task<IActionResult> Get()
    {
        var userInfo = serviceProvider.GetRequiredService<IUserInfo>();
        await serviceProvider.GetRequiredService<GCDataCollection>().CollectData(userInfo.Id);
        return Ok();
    }

    [HttpGet("status")]
    [AllowAnonymous]
    public IActionResult Status()
    {
        return Ok(new { Status = "Healthy" });
    }

    [HttpGet("app-user")]
    public async Task<IActionResult> AuthenticatedStatus()
    {
        var user = await serviceProvider.GetRequiredService<GetUserService>().GetAsync();
        return Ok(user);
    }

    [HttpGet("user-info")]
    public IActionResult GetUserinfo()
    {
        var userInfo = serviceProvider.GetRequiredService<IUserInfo>();
        return Ok(userInfo);
    }
}
