using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Plutus.Infrastructure.Services;

public class ConfirmRequisitionService(IUserInfo userInfo, AppDbContext dbContext, GCListAccounts gcListAccounts, GCDataCollection dataCollection)
{
    public async Task<Response> ConfirmAsync(string requisitionId)
    {
        var userStatus = UserStatus.RequisitionConfirmed;
        var revolutAccountId = await dbContext.Users.Where(x => x.Id == userInfo.Id).Select(x => x.RevolutAccountId).FirstOrDefaultAsync();

        if (!string.IsNullOrEmpty(revolutAccountId))
        {
            userStatus = UserStatus.RevolutConfirmed;
        }
        else
        {
            var accountIds = await gcListAccounts.GetAccountIdsAsync();
            userStatus = accountIds.Count > 1 ? UserStatus.RequisitionConfirmed : UserStatus.RevolutConfirmed;
            revolutAccountId = accountIds.Count == 1 ? accountIds[0] : null;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await dbContext.Users
                .Where(x => x.Id == userInfo.Id)
                .UpdateFromQueryAsync(x => new User
                {
                    Id = x.Id,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    UserName = x.UserName,
                    Name = x.Name,
                    Status = userStatus,
                    RevolutAccountId = revolutAccountId
                });

            await dbContext.Requisitions.Where(x => x.UserId == userInfo.Id)
                .Where(x => x.Id != requisitionId)
                .Where(x => x.IsConfirmed)
                .DeleteFromQueryAsync();

            await dbContext.Requisitions.Where(x => x.UserId == userInfo.Id)
                .Where(x => x.Id == requisitionId)
                .UpdateFromQueryAsync(x => new Requisition
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    GoCardlessRequisitionId = x.GoCardlessRequisitionId,
                    Link = x.Link,
                    Created = x.Created,
                    IsConfirmed = true
                });

            await transaction.CommitAsync();

        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            return new Response
            {
                IsRevolutConfirmed = false
            };
        }

        if (!string.IsNullOrEmpty(revolutAccountId))
        {
            await GetTransactions(userInfo.Id);
        }

        return new Response
        {
            IsRevolutConfirmed = userStatus == UserStatus.RevolutConfirmed
        };
    }

    private async Task GetTransactions(string userId)
    {
        var latestTransaction = await dbContext.Transactions
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.BookingDate)
            .Select(x => (DateTime?)x.BookingDate)
            .FirstOrDefaultAsync();

        var daysSinceLastTransaction = 730;
        if (latestTransaction.HasValue)
        {
            daysSinceLastTransaction = (DateTime.UtcNow - latestTransaction.Value).Days;
        }

        daysSinceLastTransaction = Math.Min(daysSinceLastTransaction, 730);
        await dataCollection.CollectData(userId, daysSinceLastTransaction + 10);
    }

    public class Response
    {
        public bool IsRevolutConfirmed { get; set; }
    }
}

