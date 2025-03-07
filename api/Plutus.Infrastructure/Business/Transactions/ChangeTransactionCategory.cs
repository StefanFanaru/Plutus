using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Data.Entities;
using Plutus.Infrastructure.Business;
using Plutus.Infrastructure.Helpers;

namespace Plutus.Infrastructure.Business.Transactions
{
    public class ChangeTransactionCategory(AppDbContext context)
    {
        public async Task<bool> Change(string transactionId, string categoryId)
        {
            return (await context.Transactions
                .Where(transaction => transaction.Id == transactionId)
                .UpdateFromQueryAsync(transaction => new Transaction
                {
                    Id = transaction.Id,
                    CategoryId = categoryId
                })) == 1;

        }
    }
}
