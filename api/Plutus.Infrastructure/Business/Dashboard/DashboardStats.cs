using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Abstractions;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Helpers;

namespace Plutus.Infrastructure.Business.Dashboard;

public class DashboardStats(IUserInfo userInfo, AppDbContext dbContext)
{

    public async Task<Response> GetAsync()
    {
        return new Response
        {
            BalanceDetails = await GetBalanceDetailsAsync(),
            LastTransaction = await GetLastTransactionAsync()
        };
    }

    private async Task<LastTransaction> GetLastTransactionAsync()
    {
        var lastTransaction = await dbContext.Transactions
            .ApplyUserFilter(userInfo.Id)
            .OrderByDescending(x => x.BookingDate)
            .Select(x => new LastTransaction
            {
                ObligorName = x.Obligor.Name,
                BookingDate = x.BookingDate,
                Amount = x.Amount,
                IsCredit = x.IsCredit
            })
            .FirstOrDefaultAsync();
        return lastTransaction;
    }

    private async Task<BalanceDetails> GetBalanceDetailsAsync()
    {
        var balance = await dbContext.BalanceAudits
            .ApplyUserFilter(userInfo.Id)
            .OrderByDescending(x => x.RecordedAt)
            .FirstOrDefaultAsync();

        if (balance == null)
        {
            return new BalanceDetails();
        }


        // get balance per day for the last 30 days
        // group record per day and use the last record of the day
        // Get balance per day for the last 30 days
        var balancePerDay = await dbContext.BalanceAudits
            .ApplyUserFilter(userInfo.Id)
            .Where(x => x.RecordedAt.Date > DateTime.UtcNow.AddDays(-30).Date) // Filter for the last 30 days
            .GroupBy(x => x.RecordedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Amount = g.OrderByDescending(x => x.RecordedAt).FirstOrDefault().Amount // Get the last record's amount
            })
            .OrderBy(x => x.Date)
            .ToListAsync();

        // ensure there is a record for each day
        // if there is no record for a day, use the last known previous balance to that day (fill the gaps)
        // go in reverse order to fill the gaps
        var lastKnownBalance = balance.Amount;
        for (var i = 0; i < 30; i++)
        {
            var date = DateTime.UtcNow.AddDays(-i).Date;
            if (balancePerDay.All(x => x.Date != date))
            {
                balancePerDay.Add(new
                {
                    Date = date,
                    Amount = lastKnownBalance
                });
            }
            else
            {
                lastKnownBalance = balancePerDay.First(x => x.Date == date).Amount;
            }
        }

        return new BalanceDetails
        {
            Balance = balance.Amount,
            RecordedAt = balance.RecordedAt,
            BalancePerDay = [.. balancePerDay.Select(x => x.Amount)]
        };
    }

    public class Response
    {
        public BalanceDetails BalanceDetails { get; set; }
        public LastTransaction LastTransaction { get; set; }

    }

    public class LastTransaction
    {
        public string ObligorName { get; set; }
        public DateTime BookingDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsCredit { get; set; }
    }

    public class BalanceDetails
    {
        public decimal Balance { get; set; }
        public DateTime? RecordedAt { get; set; }
        public List<decimal> BalancePerDay { get; set; } = [];
    }
}
