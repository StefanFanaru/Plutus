using Microsoft.AspNetCore.Authorization;
using Plutus.Infrastructure.Business.FixedExpenses;

namespace Plutus.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FixedExpensesController(IServiceProvider serviceProvider) : ControllerBase
{

    [HttpGet("expenses-per-month")]
    public async Task<IActionResult> GetExpensesPerMonth()
    {
        var response = await serviceProvider.GetRequiredService<FixedExpensesPerMonth>().GetAsync();
        return Ok(response);
    }
}
