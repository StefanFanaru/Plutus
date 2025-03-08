using Plutus.Infrastructure.Business;

namespace Plutus.Infrastructure.Business.Transactions
{
    public class ExcludeTransaction(IUserInfo userInfo, AppDbContext context)
    {
        public async Task<bool> SetIsExcluded(string id, bool value)
        {
            return (await context.Transactions
                .Where(transaction => transaction.Id == id)
                .ApplyUserFilter(userInfo.Id)
                .UpdateFromQueryAsync(transaction => new Transaction
                {
                    Id = transaction.Id,
                    UserId = transaction.UserId,
                    CategoryId = transaction.CategoryId,
                    IsExcluded = value
                })) == 1;

        }
    }
}
