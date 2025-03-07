using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Data.Entities;
using Plutus.Infrastructure.Enums;

namespace Plutus.Infrastructure.Services;

public class GCDataCollection(GCGetData getDataService, ILogger<GCDataCollection> logger, AppDbContext dbContext, GCInsertData insertDataService)
{

    public async Task CollectData()
    {
        await GetBalanceAsync();
        await GetTransactionsAsync();
    }

    private async Task GetTransactionsAsync()
    {
        var requestsInLast24h = await GetRequestsInLast24hAsync(GoCardlessRequestType.Trasations);

        if (requestsInLast24h >= 4)
        {
            logger.LogInformation("Too many requests in the last 24h. Skipping transactions request.");
            return;
        }

        var transactions = await getDataService.GetTransactionsAsync();
        await insertDataService.InsertData(transactions);
    }

    private async Task GetBalanceAsync()
    {
        var requestsInLast24h = await GetRequestsInLast24hAsync(GoCardlessRequestType.Balance);

        if (requestsInLast24h >= 4)
        {
            logger.LogInformation("Too many requests in the last 24h. Skipping balance request.");
            return;
        }

        var balance = await getDataService.GetBalanceAsync();
        await dbContext.BalanceAudits.AddAsync(new RevolutBalanceAudit
        {
            Id = Guid.NewGuid().ToString(),
            Amount = balance,
            RecordedAt = DateTime.UtcNow
        });

        await dbContext.GoCardlessRequests.AddAsync(new GoCardlessRequest
        {
            Id = Guid.NewGuid().ToString(),
            Type = GoCardlessRequestType.Balance,
            MadeAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }

    private async Task<int> GetRequestsInLast24hAsync(GoCardlessRequestType type)
    {
        return await dbContext.GoCardlessRequests
            .Where(x => x.Type == type)
            .Where(x => x.MadeAt > DateTime.UtcNow.AddHours(-24))
            .CountAsync();
    }
}
