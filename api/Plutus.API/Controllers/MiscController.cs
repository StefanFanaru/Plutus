using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Plutus.Infrastructure.Services;

namespace Plutus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MiscController(IServiceProvider serviceProvider) : ControllerBase
    {

        [HttpGet("get-data")]
        public async Task<IActionResult> Get()
        {
            await serviceProvider.GetRequiredService<GCDataCollection>().CollectData();
            return Ok();
        }

        [HttpGet("status")]
        [AllowAnonymous]
        public IActionResult Status()
        {
            return Ok(new { Status = "Healthy" });
        }

        [HttpGet("authenticated-status")]
        public IActionResult AuthenticatedStatus()
        {
            return Ok(new { Status = "Healthy" });
        }
    }
}
