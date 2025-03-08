namespace Plutus.Infrastructure.Data;

public class DataMigrator(AppDbContext context, IEnumerable<IDataMigration> dataMigrations)
{
    public async Task MigrateData()
    {
        var appMigrations = context.DataMigrations.Select(m => m.Name).ToList();

        foreach (var migration in dataMigrations.Where(m => !appMigrations.Contains(m.GetType().Name)))
        {
            await migration.MigrateAsync();
            InsertDatabaseMigration(migration.GetType().Name, nameof(AppDbContext));
            await context.SaveChangesAsync();
        }
    }

    private void InsertDatabaseMigration(string name, string type)
    {
        var dataMigration = new DataMigration
        {
            Name = name,
            Id = Guid.NewGuid().ToString(),
            Type = type,
            InsertTime = DateTime.UtcNow
        };
        context.DataMigrations.Add(dataMigration);
    }
}

