using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plutus.Infrastructure.Abstractions;
using Plutus.Infrastructure.Services;

namespace Plutus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiscController(IServiceProvider serviceProvider, IUserInfo userInfo) : ControllerBase
    {

        [HttpGet("get-data")]
        public async Task<IActionResult> Get()
        {
            await serviceProvider.GetRequiredService<GCDataCollection>().CollectData(userInfo.Id);
            return Ok();
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult Status()
        {
            return Ok(new { Status = "Healthy" });
        }

        [HttpGet("validate-user")]
        public async Task<IActionResult> AuthenticatedStatus()
        {
            var isValid = await serviceProvider.GetRequiredService<ValidateUser>().IsValidUser();
            return Ok(new { Status = "Valid" });
        }

        [HttpGet("user-info")]
        public IActionResult GetUserinfo()
        {
            return Ok(userInfo);
        }
    }
}
