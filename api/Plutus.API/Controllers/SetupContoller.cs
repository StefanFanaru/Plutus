namespace Plutus.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SetupController(IServiceProvider serviceProvider) : ControllerBase
{
    [HttpGet("requisition-url")]
    public async Task<IActionResult> GetRequisitionUrl()
    {
        var url = await serviceProvider.GetRequiredService<GetRequisitionUrlService>().GetUrlAsync();
        return Ok(url);
    }

    [HttpGet("confirm-requisition")]
    public async Task<IActionResult> GetRequisitionUrl(string requisitionId)
    {
        var response = await serviceProvider.GetRequiredService<ConfirmRequisitionService>().ConfirmAsync(requisitionId);
        return Ok(response);
    }

    [HttpGet("list-accounts")]
    public async Task<IActionResult> GetAccounts()
    {
        return Ok(await serviceProvider.GetRequiredService<GCListAccounts>().ListAsync());
    }

    [HttpPost("select-account")]
    public async Task<IActionResult> SelectAccount(string accountId)
    {
        var success = await serviceProvider.GetRequiredService<SaveSelectedAccount>().SelectAccountAsynct(accountId);
        return success ? Ok() : BadRequest();
    }
}
