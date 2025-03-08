namespace Plutus.Infrastructure.Business.Transactions;

public class ChangeTransactionCategory(IUserInfo userInfo, AppDbContext context)
{
    public async Task<bool> Change(string transactionId, string categoryId)
    {
        return await context.Transactions
            .Where(transaction => transaction.Id == transactionId)
            .ApplyUserFilter(userInfo.Id)
            .UpdateFromQueryAsync(transaction => new Transaction
            {
                Id = transaction.Id,
                UserId = transaction.UserId,
                CategoryId = categoryId
            }) == 1;

    }
}
