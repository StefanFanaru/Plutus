using Plutus.Infrastructure.Common;
using Plutus.Infrastructure.Data.Entities;

using Z.EntityFramework.Plus;


namespace Plutus.Infrastructure.Data.DataMigrations;

public class FixedCategoryDataMigration(AppDbContext appDbContext) : IDataMigration
{
    public async Task MigrateAsync()
    {
        await appDbContext.Transactions.Where(x => x.Obligor.IsForFixedExpenses)
            .UpdateFromQueryAsync(x => new Transaction
            {
                Id = x.Id,
                CategoryId = AppConstants.FixedCategoryId
            });
    }
}
