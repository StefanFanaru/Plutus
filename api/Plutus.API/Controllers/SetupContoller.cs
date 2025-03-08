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
        var success = await serviceProvider.GetRequiredService<ConfirmRequisitionService>().ConfirmAsync(requisitionId);
        return success ? Ok() : NotFound();
    }
}
