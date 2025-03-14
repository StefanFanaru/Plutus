namespace Plutus.Infrastructure.Business.Obligors;

public class ListObligors(IUserInfo userInfo, AppDbContext context)
{
    public async Task<ListResponse<ObligorListItem>> Get(ListRequest request)
    {
        var query = context.Obligors
            .ApplyUserFilter(userInfo.Id)
            .GroupBy(obligor => obligor.DisplayName)
            .Select(group => new ObligorListItem
            {
                DisplayName = group.Key,
                TransactionsThisMonthCount = context.Transactions.Count(t => group.Select(g => g.Id).Contains(t.ObligorId) && t.BookingDate.Date >= DateTime.UtcNow.AddDays(-30)),
                TotalTransactionsCount = context.Transactions.Count(t => group.Select(g => g.Id).Contains(t.ObligorId)),
                AmmountCreditedThisMonth = context.Transactions.Where(t => group.Select(g => g.Id).Contains(t.ObligorId) && t.IsCredit && t.BookingDate.Date >= DateTime.UtcNow.AddDays(-30)).Sum(t => t.Amount),
                TotalAmmountCredited = context.Transactions.Where(t => group.Select(g => g.Id).Contains(t.ObligorId) && t.IsCredit).Sum(t => t.Amount),
                LatestTransaction = context.Transactions.Where(t => group.Select(g => g.Id).Contains(t.ObligorId)).OrderByDescending(t => t.BookingDate).Select(t => t.BookingDate).FirstOrDefault(),
                IsForFixedExpenses = group.First().IsForFixedExpenses,
            })
            .Where(obligor => obligor.TotalTransactionsCount > 0)
            .ApplyFilter(request.Filter);

        var pagedQuery = query
            .ApplySorting(request.SortField, request.SortOrder)
            .ApplyPaging(request.PageNumber, request.PageSize);

        return new ListResponse<ObligorListItem>
        {
            Items = await pagedQuery.ToListAsync(),
            TotalCount = await query.CountAsync()
        };
    }

    public async Task<List<ObligorsCreditedPerMonthItem>> GetAmmountCreditedPerMonth(List<string> displayNames)
    {
        var ammountCreditedPerMonth = await context.Transactions
            .Where(t => displayNames.Contains(t.Obligor.DisplayName))
            .Where(t => t.BookingDate >= DateTime.UtcNow.AddMonths(-11))
            .GroupBy(t => new { t.Obligor.DisplayName, t.BookingDate.Year, t.BookingDate.Month })
            .Select(g => new { g.Key.DisplayName, g.Key.Month, Sum = g.Sum(t => t.Amount) })
            .ToListAsync();

        var ammountCreditedPerMonthItems = ammountCreditedPerMonth
            .GroupBy(t => t.DisplayName)
            .Select(g => new ObligorsCreditedPerMonthItem
            {
                ObligorDisplayName = g.Key,
                AmmountCreditedPerMonth = g.ToDictionary(t => t.Month, t => t.Sum)
            }).ToList();

        // fill with 0 for missing months
        foreach (var item in ammountCreditedPerMonthItems)
        {
            for (var i = 1; i <= 12; i++)
            {
                item.AmmountCreditedPerMonth.TryAdd(i, 0);
            }
        }

        return ammountCreditedPerMonthItems;
    }


    public class ObligorListItem
    {
        public string DisplayName { get; set; }
        public int TransactionsThisMonthCount { get; set; }
        public int TotalTransactionsCount { get; set; }
        public decimal AmmountCreditedThisMonth { get; set; }
        public decimal TotalAmmountCredited { get; set; }
        public bool IsForFixedExpenses { get; set; }
        public DateTime LatestTransaction { get; set; }
    }

    public class ObligorsCreditedPerMonthItem
    {
        public string ObligorDisplayName { get; set; }
        public Dictionary<int, decimal> AmmountCreditedPerMonth { get; set; }
    }
}
