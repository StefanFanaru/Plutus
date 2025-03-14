using Plutus.Infrastructure.Business.Transactions;

namespace Plutus.Infrastructure.Business.FixedExpenses;

public class FixedExpensesPerMonth(IUserInfo userInfo, AppDbContext context)
{
    private static readonly string[] Months =
    [
      "Jan",
      "Feb",
      "Mar",
      "Apr",
      "May",
      "Jun",
      "Jul",
      "Aug",
      "Sep",
      "Oct",
      "Nov",
      "Dec",
    ];

    public async Task<List<ResponseItem>> GetAsync()
    {
        var expensesGrouped = await context.Transactions
            .ApplyUserFilter(userInfo.Id)
            .Where(x => !x.IsExcluded)
            .Where(transaction => !transaction.IsSplit)
            .Where(x => x.IsCredit)
            .Where(x => x.Obligor.IsForFixedExpenses)
            .Where(x => (x.BookingDate.Month > DateTime.UtcNow.AddMonths(-12).Month && x.BookingDate.Year == DateTime.UtcNow.Year - 1) || x.BookingDate.Month <= DateTime.UtcNow.AddMonths(-12).Month && x.BookingDate.Year == DateTime.UtcNow.Year)
            .GroupBy(x => new
            {
                x.BookingDate.Year,
                x.BookingDate.Month
            })
            .OrderBy(x => x.Key.Month)
            .Select(g => new ResponseItem
            {
                MonthName = Months[g.Key.Month - 1],
                Year = g.Key.Year,
                Value = g.Sum(x => x.Amount),
                Transactions = g.Select(x => new TransactionListItem
                {
                    Id = x.Id,
                    Amount = x.Amount,
                    BookedAt = x.BookingDate,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name,
                    ObligorId = x.ObligorId,
                    ObligorName = x.Obligor.Name,
                    ObligorDisplayName = x.Obligor.DisplayName,
                    Type = x.Type,
                }).ToList()
            })

            .ToListAsync();

        return expensesGrouped;
    }

    public class ResponseItem
    {
        public decimal Value { get; set; }
        public int Year { get; set; }
        public string MonthName { get; set; }
        public List<TransactionListItem> Transactions { get; set; }
    }
}
