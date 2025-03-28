namespace Plutus.Infrastructure.Business.Dashboard;

public class DashboardSpendingThisWeek(IUserInfo userInfo, AppDbContext dbContext)
{
    public async Task<Response> GetAsync(Request request)
    {
        var categories = await dbContext.Categories.ToListAsync();

        var transactions = Query(request.StartDate, request.EndDate)
            .Select(x => new
            {
                x.CategoryId,
                x.Amount,
                x.BookingDate
            });

        if (!await transactions.AnyAsync())
        {
            return new Response();
        }

        var result = new List<SpentByCategoryItem>();
        var days = (request.EndDate - request.StartDate).Days;

        foreach (var category in categories)
        {
            var data = new List<decimal>();
            for (var i = 0; i <= days; i++)
            {
                data.Add(await transactions
                    .Where(x => x.CategoryId == category.Id)
                    .Where(x => x.BookingDate.Date == DateTime.UtcNow.AddDays(-i).Date)
                    .SumAsync(x => x.Amount));
            }

            data.Reverse();

            result.Add(new SpentByCategoryItem
            {
                CategoryID = category.Id,
                CategoryName = category.Name,
                Data = data
            });
        }

        result = [.. result.Where(x => x.Data.Sum() != 0)];

        var spentLastIntevalDays = await transactions.SumAsync(x => x.Amount);
        var spentLastLastIntevalDays =await Query(request.StartDate.AddDays(-days), request.StartDate.Date.AddDays(-1)).SumAsync(x => x.Amount);

        return new Response
        {
            SpentByCategoryItems = result,
            TotalSpent = spentLastIntevalDays,
            PercentageSpendingChange = (int)Math.Round((spentLastIntevalDays - spentLastLastIntevalDays) / spentLastLastIntevalDays * 100)
        };
    }

    private IQueryable<Transaction> Query(DateTime startDate, DateTime endDate)
    {
        return dbContext.Transactions
            .ApplyUserFilter(userInfo.Id)
            .Where(x => x.BookingDate.Date >= startDate.Date && x.BookingDate.Date <= endDate.Date)
            .Where(x => !x.Obligor.IsForFixedExpenses)
            .Where(x => !x.IsExcluded)
            .Where(x => x.IsCredit);
    }

    public class SpentByCategoryItem
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public List<decimal> Data { get; set; }
    }

    public class Request
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class Response
    {
        public List<SpentByCategoryItem> SpentByCategoryItems { get; set; } = [];
        public decimal TotalSpent { get; set; }
        public decimal PercentageSpendingChange { get; set; }
    }
}
