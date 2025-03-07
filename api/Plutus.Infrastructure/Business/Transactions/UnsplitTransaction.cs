using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Data.Entities;

namespace Plutus.Infrastructure.Business.Transactions
{
    public class UnsplitTransaction(AppDbContext context)
    {
        public async Task<bool> Unsplit(string transactionId)
        {
            var originalTransactionId = await context.Transactions
                .Where(transaction => transaction.Id == transactionId)
                .Select(transaction => transaction.OriginalTransactionId)
                .SingleAsync();

            if (originalTransactionId == null)
            {
                return false;
            }

            var splits = await context.Transactions
                .Where(transaction => transaction.OriginalTransactionId != null && transaction.OriginalTransactionId == originalTransactionId)
                .ToListAsync();


            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Transactions.RemoveRange(splits);

                    await context.Transactions
                        .Where(transaction => transaction.Id == originalTransactionId)
                        .UpdateFromQueryAsync(transaction => new Transaction
                        {
                            Id = transaction.Id,
                            CategoryId = transaction.CategoryId,
                            IsExcluded = false,
                            IsSplit = false
                        });

                    await context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return true;

                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    return false;
                }
            }
        }
    }
}
