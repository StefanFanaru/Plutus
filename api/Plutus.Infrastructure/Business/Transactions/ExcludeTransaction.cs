using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Abstractions;
using Plutus.Infrastructure.Data.Entities;
using Plutus.Infrastructure.Business;
using Plutus.Infrastructure.Helpers;

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
