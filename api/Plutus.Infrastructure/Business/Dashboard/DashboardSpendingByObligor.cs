namespace Plutus.Infrastructure.Business.Dashboard;

public class DashboardSpendingByObligor(IUserInfo userInfo, AppDbContext dbContext)
{
    public async Task<Response> GetAsync()
    {
        var transactions = await dbContext.Transactions
            .ApplyUserFilter(userInfo.Id)
            .Where(x => x.BookingDate.Date >= DateTime.UtcNow.AddDays(-30))
            .Where(x => !x.Obligor.IsForFixedExpenses)
            .Where(x => x.IsCredit)
            .Where(x => !x.IsExcluded)
            .Select(x => new { x.Amount, x.ObligorId })
            .ToListAsync();

        var obligorIds = transactions.Select(x => x.ObligorId).Distinct();
        var obligors = await dbContext.Obligors.Where(x => obligorIds.Contains(x.Id)).ToListAsync();

        var totalSpent = transactions.Sum(x => x.Amount);
        var response = new Response
        {
            Items = []
        };

        foreach (var obligor in obligors)
        {
            var spent = transactions.Where(x => x.ObligorId == obligor.Id).Sum(x => x.Amount);
            var percentage = totalSpent == 0 ? 0 : Math.Round(spent / totalSpent * 100, 2);

            response.Items.Add(new SpentByObligorItem
            {
                ObligorId = obligor.Id,
                ObligorName = obligor.DisplayName,
                Amount = spent,
                Percentage = percentage
            });
        }

        response.Items = [.. response.Items.OrderBy(x => x.Amount)];

        // take the top 6 categories and sum the rest into an "Other" category
        if (response.Items.Count > 4)
        {
            var other = response.Items.Skip(4).Sum(x => x.Amount);
            response.Items = [.. response.Items.Take(4)];
            response.Items.Add(new SpentByObligorItem
            {
                ObligorId = "Other",
                ObligorName = "Other",
                Amount = other,
                Percentage = totalSpent == 0 ? 0 : Math.Round(other / totalSpent * 100, 2)
            });
        }

        return response;
    }

    public class SpentByObligorItem
    {
        public string ObligorId { get; set; }
        public string ObligorName { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class Response
    {
        public List<SpentByObligorItem> Items { get; set; } = [];
    }
}
