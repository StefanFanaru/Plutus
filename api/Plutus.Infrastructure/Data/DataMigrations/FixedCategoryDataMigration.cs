namespace Plutus.Infrastructure.Data.DataMigrations;

public class FixedCategoryDataMigration(AppDbContext appDbContext) : IDataMigration
{
    public async Task MigrateAsync()
    {
        await appDbContext.Transactions.Where(x => x.Obligor.IsForFixedExpenses)
            .UpdateFromQueryAsync(x => new Transaction
            {
                Id = x.Id,
                UserId = x.UserId,
                CategoryId = AppConstants.FixedCategoryId
            });
    }
}
