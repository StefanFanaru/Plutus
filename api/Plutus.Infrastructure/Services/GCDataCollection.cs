using Microsoft.Extensions.Logging;

namespace Plutus.Infrastructure.Services;

public class GCDataCollection(GCGetData getDataService, ILogger<GCDataCollection> logger, AppDbContext dbContext, GCInsertData insertDataService)
{
    public async Task CollectData(string userId, int daysBack = 7)
    {
        var accountId = await dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => x.RevolutAccountId)
            .FirstOrDefaultAsync();
        await GetBalanceAsync(accountId, userId);
        await GetTransactionsAsync(accountId, userId, daysBack);
    }

    public async Task GetTransactionsAsync(string accountId, string userId, int daysBack)
    {
        var requestsInLast24h = await GetRequestsInLast24hAsync(GoCardlessRequestType.Trasations, userId);

        if (requestsInLast24h >= 4)
        {
            logger.LogInformation("Too many requests in the last 24h. Skipping transactions request.");
            return;
        }

        var transactions = await getDataService.GetTransactionsAsync(daysBack, accountId);
        if (transactions != null && transactions.Transactions.Booked.Count != 0)
        {
            await insertDataService.InsertData(transactions, userId);
        }

        await dbContext.GoCardlessRequests.AddAsync(new GoCardlessRequest
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Type = GoCardlessRequestType.Trasations,
            MadeAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }

    private async Task GetBalanceAsync(string accountId, string userId)
    {
        var requestsInLast24h = await GetRequestsInLast24hAsync(GoCardlessRequestType.Balance, userId);

        if (requestsInLast24h >= 4)
        {
            logger.LogInformation("Too many requests in the last 24h. Skipping balance request.");
            return;
        }

        var balance = await getDataService.GetBalanceAsync(accountId);
        await dbContext.BalanceAudits.AddAsync(new RevolutBalanceAudit
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Amount = balance,
            RecordedAt = DateTime.UtcNow
        });

        await dbContext.GoCardlessRequests.AddAsync(new GoCardlessRequest
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Type = GoCardlessRequestType.Balance,
            MadeAt = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();
    }

    private async Task<int> GetRequestsInLast24hAsync(GoCardlessRequestType type, string userId)
    {
        return await dbContext.GoCardlessRequests
            .Where(x => x.UserId == userId)
            .Where(x => x.Type == type)
            .Where(x => x.MadeAt > DateTime.UtcNow.AddHours(-24))
            .CountAsync();
    }
}
