using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Plutus.Infrastructure.Services;

public class ConfirmRequisitionService(IUserInfo userInfo, AppDbContext dbContext)
{
    public async Task<bool> ConfirmAsync(string requisitionId)
    {
        return await dbContext.Requisitions.Where(x => x.UserId == userInfo.Id)
            .Where(x => x.Id == requisitionId)
            .UpdateFromQueryAsync(x => new Requisition
            {
                Id = x.Id,
                UserId = x.UserId,
                GoCardlessRequisitionId = x.GoCardlessRequisitionId,
                Created = x.Created,
                IsConfirmed = true
            }) == 1;

    }
}

