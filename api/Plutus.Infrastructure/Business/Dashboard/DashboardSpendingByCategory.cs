namespace Plutus.Infrastructure.Business.Dashboard;

public class DashboardSpendingByCategory(IUserInfo userInfo, AppDbContext dbContext)
{
    public async Task<Response> GetAsync()
    {
        var categories = await dbContext.Categories.ToListAsync();

        var transactions = await dbContext.Transactions
            .ApplyUserFilter(userInfo.Id)
            .Where(x => x.BookingDate.Date >= DateTime.UtcNow.AddDays(-30))
            .Where(x => !x.Obligor.IsForFixedExpenses)
            .Where(x => x.IsCredit)
            .Where(x => !x.IsExcluded)
            .Select(x => new { x.Amount, x.CategoryId })
            .ToListAsync();

        if (transactions.Count == 0)
        {
            return new Response();
        }

        var totalSpent = transactions.Sum(x => x.Amount);
        var response = new Response
        {
            Items = []
        };

        foreach (var category in categories)
        {
            var spent = transactions.Where(x => x.CategoryId == category.Id).Sum(x => x.Amount);
            var percentage = totalSpent == 0 ? 0 : Math.Round(spent / totalSpent * 100, 2);

            response.Items.Add(new SpentByCategoryItem
            {
                CategoryId = category.Id,
                CategoryName = category.Name,
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
            response.Items.Add(new SpentByCategoryItem
            {
                CategoryId = "Other",
                CategoryName = "Other",
                Amount = other,
                Percentage = totalSpent == 0 ? 0 : Math.Round(other / totalSpent * 100, 2)
            });
        }

        return response;
    }

    public class SpentByCategoryItem
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    public class Response
    {
        public List<SpentByCategoryItem> Items { get; set; } = [];
    }
}
