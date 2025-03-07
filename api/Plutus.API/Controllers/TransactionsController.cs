using Microsoft.AspNetCore.Mvc;
using Plutus.Infrastructure.Business.Transactions;
using Plutus.Infrastructure.Dtos;

namespace Plutus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(IServiceProvider serviceProvider) : ControllerBase
    {
        [HttpPost("list")]
        public async Task<IActionResult> List(ListRequest request)
        {
            return Ok(await serviceProvider.GetRequiredService<ListTransactions>().Get(request));
        }

        [HttpPost("exclude")]
        public async Task<IActionResult> ChangeIsExcluded(string id, bool value)
        {
            var success = await serviceProvider.GetRequiredService<ExcludeTransaction>().SetIsExcluded(id, value);
            return success ? Ok() : NotFound();
        }

        [HttpPost("change-category")]
        public async Task<IActionResult> ChangeCategory(string transactionId, string categoryId)
        {
            var success = await serviceProvider.GetRequiredService<ChangeTransactionCategory>().Change(transactionId, categoryId);
            return success ? Ok() : NotFound();
        }

        [HttpPost("split")]
        public async Task<IActionResult> Split(SplitTransaction.Request request)
        {
            var success = await serviceProvider.GetRequiredService<SplitTransaction>().Split(request);
            return success ? Ok() : NotFound();
        }

        [HttpPost("unsplit")]
        public async Task<IActionResult> Unsplit(string transactionId)
        {
            var success = await serviceProvider.GetRequiredService<UnsplitTransaction>().Unsplit(transactionId);
            return success ? Ok() : NotFound();
        }
    }
}
