namespace Plutus.Infrastructure.Business.Dashboard;

public class DashboardSpendingStats(IUserInfo userInfo, AppDbContext dbContext)
{
    public async Task<Response> GetAsync()
    {
        var spentLast30Days = await QueryTransactions(50, 0, 30).ToListAsync();

        if (spentLast30Days.Count == 0)
        {
            return new Response();
        }

        var spentLast30DaysExact = await SumTransactionsInInterval(30, 0);
        var spentLastLast30DaysExact = await SumTransactionsInInterval(60, 31);

        var response = new Response
        {
            SpentPerDayLast25Days = [.. spentLast30Days.Take(25)],
            ProjectionNext5Days = MakeProjection([.. spentLast30Days.Select(x => x.Amount)]),
            TotalSpendLast30Days = spentLast30DaysExact,
            PercentageSpendingChange = (int)Math.Round((spentLast30DaysExact - spentLastLast30DaysExact) / spentLastLast30DaysExact * 100)
        };

        return response;
    }

    private IQueryable<Response.SpentPerDay> QueryTransactions(int daysBack, int skip, int take)
    {
        return dbContext.Transactions
            .ApplyUserFilter(userInfo.Id)
            .Where(x => x.BookingDate.Date >= DateTime.UtcNow.AddDays(-daysBack))
            .Where(x => !x.Obligor.IsForFixedExpenses)
            .Where(x => x.IsCredit)
            .Where(x => !x.IsExcluded)
            .Where(x => !x.IsExcluded)
            .GroupBy(x => x.BookingDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                Amount = g.Sum(x => x.Amount)
            })
            .OrderByDescending(x => x.Date)
            .Skip(skip)
            .Take(take)
            .Select(x => new Response.SpentPerDay
            {
                Date = x.Date,
                Amount = x.Amount
            });
    }

    private Task<decimal> SumTransactionsInInterval(int daysBack, int daysBackLimit)
    {
        return dbContext.Transactions
            .ApplyUserFilter(userInfo.Id)
            .Where(x => x.BookingDate.Date >= DateTime.UtcNow.AddDays(-daysBack).Date && x.BookingDate.Date <= DateTime.UtcNow.AddDays(-daysBackLimit).Date)
            .Where(x => !x.Obligor.IsForFixedExpenses)
            .Where(x => !x.IsExcluded)
            .Where(x => x.IsCredit)
            .Include(x => x.Obligor)
            .Select(x => new TransacitonAmmount
            {
                Amount = x.Amount,
                BookingDate = x.BookingDate
            })
            .SumAsync(x => x.Amount);
    }
    public static decimal GetMedian(List<decimal> numbers)
    {
        if (numbers == null || numbers.Count == 0)
        {
            throw new ArgumentException("The list cannot be null or empty.");
        }

        var sortedNumbers = numbers.OrderBy(n => n).ToList();
        var count = sortedNumbers.Count;
        decimal median;

        if (count % 2 == 0)
        {
            median = (sortedNumbers[count / 2 - 1] + sortedNumbers[count / 2]) / 2;
        }
        else
        {
            median = sortedNumbers[count / 2];
        }

        return median;
    }

    private static List<Response.SpentPerDay> MakeProjection(List<decimal> inputValues)
    {
        var median = GetMedian(inputValues);

        var projection = new List<Response.SpentPerDay>();
        for (var i = 1; i <= 5; i++)
        {
            projection.Add(new Response.SpentPerDay
            {
                Date = DateTime.UtcNow.AddDays(i),
                Amount = median
            });
        }

        return projection;
    }

    public class TransacitonAmmount
    {
        public decimal Amount { get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class Response
    {
        public List<SpentPerDay> SpentPerDayLast25Days { get; set; } = [];
        public List<SpentPerDay> ProjectionNext5Days { get; set; } = [];
        public decimal TotalSpendLast30Days { get; set; }
        public int PercentageSpendingChange { get; set; }

        public class SpentPerDay
        {
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
