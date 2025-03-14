using Plutus.Infrastructure.Business.Dashboard;

namespace Plutus.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController(IServiceProvider serviceProvider) : ControllerBase
{

    [HttpGet("stats-cards")]
    public async Task<IActionResult> GetStats()
    {
        return Ok(await serviceProvider.GetRequiredService<DashboardStatCards>().GetAsync());
    }

    [HttpGet("spending-stats")]
    public async Task<IActionResult> GetSpendingStats()
    {
        return Ok(await serviceProvider.GetRequiredService<DashboardSpendingStats>().GetAsync());
    }

    [HttpGet("spending-by-category")]
    public async Task<IActionResult> GetSpendingByCategory()
    {
        return Ok(await serviceProvider.GetRequiredService<DashboardSpendingByCategory>().GetAsync());
    }

    [HttpGet("spending-by-obligor")]
    public async Task<IActionResult> GetSpendingByObligor()
    {
        return Ok(await serviceProvider.GetRequiredService<DashboardSpendingByObligor>().GetAsync());
    }

    [HttpPost("spending-by-period")]
    public async Task<IActionResult> GetSpendingByPeriod(DashboardSpendingThisWeek.Request request)
    {
        return Ok(await serviceProvider.GetRequiredService<DashboardSpendingThisWeek>().GetAsync(request));
    }
}
