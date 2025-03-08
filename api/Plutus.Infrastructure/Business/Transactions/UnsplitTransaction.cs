namespace Plutus.Infrastructure.Business.Transactions;

public class UnsplitTransaction(IUserInfo userInfo, AppDbContext context)
{
    public async Task<bool> Unsplit(string transactionId)
    {
        var originalTransactionId = await context.Transactions
            .ApplyUserFilter(userInfo.Id)
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


        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            context.Transactions.RemoveRange(splits);

            await context.Transactions
                .Where(transaction => transaction.Id == originalTransactionId)
                .UpdateFromQueryAsync(transaction => new Transaction
                {
                    Id = transaction.Id,
                    UserId = transaction.UserId,
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
