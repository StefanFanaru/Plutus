using Plutus.Infrastructure.Services;

namespace Plutus.Infrastructure.Data.DataMigrations;

public class IndexObligorsMigration4(AppDbContext appDbContext) : IDataMigration
{
    public async Task MigrateAsync()
    {
        var obligors = await appDbContext.Obligors.ToListAsync();
        foreach (var obligor in obligors)
        {
            obligor.DisplayName = ObligorIndexer.GetDisplayName(obligor.Name);
            appDbContext.Update(obligor);
        }

        await appDbContext.SaveChangesAsync();
    }
}
