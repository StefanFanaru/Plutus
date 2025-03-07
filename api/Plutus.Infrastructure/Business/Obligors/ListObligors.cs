using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Helpers;
using Plutus.Infrastructure.Dtos;

namespace Plutus.Infrastructure.Business.Obligors
{
    public class ListObligors(AppDbContext context)
    {
        public async Task<ListResponse<ObligorListItem>> Get(ListRequest request)
        {
            var query = context.Obligors.AsQueryable();
            var mappedQuery = query
                .Select(obligor => new ObligorListItem
                {
                    Id = obligor.Id,
                    Name = obligor.Name,
                    TransactionsThisMonthCount = context.Transactions.Count(t => t.ObligorId == obligor.Id && t.BookingDate.Date >= DateTime.UtcNow.AddDays(-30)),
                    TotalTransactionsCount = context.Transactions.Count(t => t.ObligorId == obligor.Id),
                    AmmountCreditedThisMonth = context.Transactions.Where(t => t.ObligorId == obligor.Id && t.IsCredit && t.BookingDate.Date >= DateTime.UtcNow.AddDays(-30)).Sum(t => t.Amount),
                    TotalAmmountCredited = context.Transactions.Where(t => t.ObligorId == obligor.Id && t.IsCredit).Sum(t => t.Amount),
                    LatestTransaction = context.Transactions.Where(t => t.ObligorId == obligor.Id).OrderByDescending(t => t.BookingDate).Select(t => t.BookingDate).FirstOrDefault(),
                    IsForFixedExpenses = obligor.IsForFixedExpenses

                })
                .Where(obligor => obligor.TotalTransactionsCount > 0)
                .ApplySorting(request.SortField, request.SortOrder)
                .ApplyFilter(request.Filter)
                .ApplyPaging(request.PageNumber, request.PageSize);

            return new ListResponse<ObligorListItem>
            {
                Items = await mappedQuery.ToListAsync(),
                TotalCount = await context.Obligors.CountAsync()
            };

        }

        public async Task<List<ObligorsCreditedPerMonthItem>> GetAmmountCreditedPerMonth(List<string> ids)
        {
            var ammountCreditedPerMonth = await context.Transactions
                .Where(t => ids.Contains(t.ObligorId))
                .Where(t => t.BookingDate >= DateTime.UtcNow.AddMonths(-11))
                .GroupBy(t => new { t.ObligorId, t.BookingDate.Year, t.BookingDate.Month })
                .Select(g => new { g.Key.ObligorId, g.Key.Month, Sum = g.Sum(t => t.Amount) })
                .ToListAsync();

            var ammountCreditedPerMonthItems = ammountCreditedPerMonth
                .GroupBy(t => t.ObligorId)
                .Select(g => new ObligorsCreditedPerMonthItem
                {
                    ObligorId = g.Key,
                    AmmountCreditedPerMonth = g.ToDictionary(t => t.Month, t => t.Sum)
                }).ToList();

            // fill with 0 for missing months
            foreach (var item in ammountCreditedPerMonthItems)
            {
                for (int i = 1; i <= 12; i++)
                {
                    item.AmmountCreditedPerMonth.TryAdd(i, 0);
                }
            }

            return ammountCreditedPerMonthItems;
        }


        public class ObligorListItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int TransactionsThisMonthCount { get; set; }
            public int TotalTransactionsCount { get; set; }
            public decimal AmmountCreditedThisMonth { get; set; }
            public decimal TotalAmmountCredited { get; set; }
            public bool IsForFixedExpenses { get; set; }
            public DateTime LatestTransaction { get; set; }
        }

        public class ObligorsCreditedPerMonthItem
        {
            public string ObligorId { get; set; }
            public Dictionary<int, decimal> AmmountCreditedPerMonth { get; set; }
        }
    }
}
