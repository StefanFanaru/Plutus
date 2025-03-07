using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Helpers;
using Plutus.Infrastructure.Dtos;

namespace Plutus.Infrastructure.Business.Categories
{
    public class ListCategories(AppDbContext context)
    {
        public async Task<List<CategoryDto>> GetAll()
        {
            return await context.Categories.AsQueryable()
                .Select(category => new CategoryDto
                {
                    Id = category.Id,
                    Name = category.Name,
                }).ToListAsync();

        }
        public async Task<ListResponse<CategoriesListItem>> Get(ListRequest request)
        {
            var query = context.Categories.AsQueryable();
            var mappedQuery = query
                .Select(category => new CategoriesListItem
                {
                    Id = category.Id,
                    Name = category.Name,
                    AmmountCreditedThisMonth = context.Transactions.Where(t => t.CategoryId == category.Id && t.IsCredit && t.BookingDate.Date >= DateTime.UtcNow.AddDays(-30)).Sum(t => t.Amount),
                    TotalAmmountCredited = context.Transactions.Where(t => t.CategoryId == category.Id && t.IsCredit).Sum(t => t.Amount),
                    LatestTransaction = context.Transactions.Where(t => t.CategoryId == category.Id).OrderByDescending(t => t.BookingDate).Select(t => t.BookingDate).FirstOrDefault(),
                })
                .ApplySorting(request.SortField, request.SortOrder)
                .ApplyFilter(request.Filter)
                .ApplyPaging(request.PageNumber, request.PageSize);

            return new ListResponse<CategoriesListItem>
            {
                Items = await mappedQuery.ToListAsync(),
                TotalCount = await context.Categories.CountAsync()
            };

        }

        public async Task<List<CategoryCreditedPerMonthItem>> GetAmmountCreditedPerMonth(List<string> ids)
        {
            var ammountCreditedPerMonth = await context.Transactions
                .Where(t => ids.Contains(t.CategoryId))
                .Where(t => t.BookingDate >= DateTime.UtcNow.AddMonths(-11))
                .GroupBy(t => new { t.CategoryId, t.BookingDate.Year, t.BookingDate.Month })
                .Select(g => new { g.Key.CategoryId, g.Key.Month, Sum = g.Sum(t => t.Amount) })
                .ToListAsync();

            var ammountCreditedPerMonthItems = ammountCreditedPerMonth
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategoryCreditedPerMonthItem
                {
                    CategoryId = g.Key,
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
        public class CategoriesListItem
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal AmmountCreditedThisMonth { get; set; }
            public decimal TotalAmmountCredited { get; set; }
            public DateTime LatestTransaction { get; set; }
        }

        public class CategoryCreditedPerMonthItem
        {
            public string CategoryId { get; set; }
            public Dictionary<int, decimal> AmmountCreditedPerMonth { get; set; }
        }

        public class CategoryDto
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
