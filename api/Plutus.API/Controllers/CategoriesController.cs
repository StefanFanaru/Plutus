using Plutus.Infrastructure.Business.Categories;
using Plutus.Infrastructure.Dtos;

namespace Plutus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController(IServiceProvider serviceProvider) : ControllerBase
    {

        [HttpPost("list")]
        public async Task<IActionResult> List(ListRequest request)
        {
            return Ok(await serviceProvider.GetRequiredService<ListCategories>().Get(request));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await serviceProvider.GetRequiredService<ListCategories>().GetAll());
        }

        [HttpPost("month-ammount")]
        public async Task<IActionResult> ListAmmountPerMonth(List<string> ids)
        {
            return Ok(await serviceProvider.GetRequiredService<ListCategories>().GetAmmountCreditedPerMonth(ids));
        }
    }
}
