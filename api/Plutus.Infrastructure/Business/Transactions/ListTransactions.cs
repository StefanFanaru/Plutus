namespace Plutus.Infrastructure.Business.Transactions
{
    public class ListTransactions(IUserInfo userInfo, AppDbContext context, IDateFilterInfo dateFilterInfo)
    {
        public async Task<ListResponse<TransactionListItem>> Get(ListRequest request)
        {
            var query = context.Transactions
                .ApplyUserFilter(userInfo.Id)
                .Where(transaction => !transaction.IsSplit)
                .Select(transaction => new TransactionListItem
                {
                    Id = transaction.Id,
                    BookedAt = transaction.BookingDate,
                    Amount = transaction.Amount,
                    Type = transaction.Type,
                    ObligorId = transaction.ObligorId,
                    ObligorName = transaction.Obligor.Name,
                    CategoryId = transaction.CategoryId,
                    CategoryName = transaction.Category.Name,
                    IsExcluded = transaction.IsExcluded,
                    IsSplitItem = transaction.OriginalTransactionId != null,
                    IsFixedExpense = transaction.Obligor.IsForFixedExpenses
                })
                .ApplyFilter(request.Filter)
                .ApplyDateFilter(nameof(TransactionListItem.BookedAt), dateFilterInfo)
                .AsQueryable();

            var pagedQuery = query
                .ApplySorting(request.SortField, request.SortOrder)
                    .ThenBy(transaction => transaction.Amount)
                .ApplyPaging(request.PageNumber, request.PageSize);

            return new ListResponse<TransactionListItem>
            {
                Items = await pagedQuery.ToListAsync(),
                TotalCount = await query.CountAsync()
            };

        }

        public class TransactionListItem
        {
            public string Id { get; set; }
            public DateTime BookedAt { get; set; }
            public decimal Amount { get; set; }
            public TransactionType? Type { get; set; }
            public string ObligorId { get; set; }
            public string ObligorName { get; set; }
            public string CategoryId { get; set; }
            public string CategoryName { get; set; }
            public bool IsExcluded { get; set; }
            public bool IsSplitItem { get; set; }
            public bool IsFixedExpense { get; set; }
        }
    }
}
