using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Data.Entities;

namespace Plutus.Infrastructure.Business.Transactions
{
    public class SplitTransaction(AppDbContext context)
    {
        public async Task<bool> Split(Request request)
        {
            var sumOfSplits = request.Splits.Sum(split => split.Amount);
            var originalTransaction = await context.Transactions
                .Where(transaction => transaction.Id == request.TransactionId)
                .SingleAsync();

            if (sumOfSplits > Math.Abs(originalTransaction.Amount) || sumOfSplits <= 0)
            {
                return false;
            }

            var isNegativeAmount = originalTransaction.Amount < 0;

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var split in request.Splits.Where(split => split.Amount > 0))
                    {
                        var newTransaction = new Transaction
                        {
                            Id = Guid.NewGuid().ToString(),
                            BookingDate = originalTransaction.BookingDate,
                            Amount = isNegativeAmount ? -split.Amount : split.Amount,
                            Type = originalTransaction.Type,
                            ObligorId = originalTransaction.ObligorId,
                            CategoryId = split.CategoryId,
                            IsCredit = originalTransaction.IsCredit,
                            IsExcluded = originalTransaction.IsExcluded,
                            OriginalTransactionId = originalTransaction.Id
                        };

                        context.Transactions.Add(newTransaction);
                    }

                    var totalSplitAmount = request.Splits.Sum(split => split.Amount);
                    var remainingAmount = Math.Abs(originalTransaction.Amount) - totalSplitAmount;

                    // create a new transaction for the remaining amount
                    if (remainingAmount != 0)
                    {
                        var newTransaction = new Transaction
                        {
                            Id = Guid.NewGuid().ToString(),
                            BookingDate = originalTransaction.BookingDate,
                            Amount = isNegativeAmount ? -remainingAmount : remainingAmount,
                            Type = originalTransaction.Type,
                            ObligorId = originalTransaction.ObligorId,
                            CategoryId = originalTransaction.CategoryId,
                            IsCredit = originalTransaction.IsCredit,
                            IsExcluded = originalTransaction.IsExcluded,
                            OriginalTransactionId = originalTransaction.Id
                        };

                        context.Transactions.Add(newTransaction);
                    }

                    originalTransaction.IsSplit = true;
                    originalTransaction.IsExcluded = true;

                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return true;
                }
                catch
                {
                    transaction.Rollback();
                    return false;
                }
            }

        }

        public class Request
        {
            public string TransactionId { get; set; }
            public List<TransactionSplitItem> Splits { get; set; }
        }

        public class TransactionSplitItem
        {
            public decimal Amount { get; set; }
            public string CategoryId { get; set; }
        }
    }
}
