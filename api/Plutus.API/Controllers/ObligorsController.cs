using Plutus.Infrastructure.Business.Obligors;
using Plutus.Infrastructure.Dtos;


namespace Plutus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ObligorsController(IServiceProvider serviceProvider) : ControllerBase
    {

        [HttpPost("list")]
        public async Task<IActionResult> List(ListRequest request)
        {
            return Ok(await serviceProvider.GetRequiredService<ListObligors>().Get(request));
        }

        [HttpPost("month-ammount")]
        public async Task<IActionResult> ListAmmountPerMonth(List<string> ids)
        {
            return Ok(await serviceProvider.GetRequiredService<ListObligors>().GetAmmountCreditedPerMonth(ids));
        }

        [HttpPost("set-fixed-expense")]
        public async Task<IActionResult> ListAmmountPerMonth(string obligorId, bool value)
        {
            var success = await serviceProvider.GetRequiredService<ChangeObligorFixedExpense>().Set(obligorId, value);
            return success ? Ok() : NotFound();
        }
    }
}
