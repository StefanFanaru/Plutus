namespace Plutus.Infrastructure.Data;

public interface IDataMigration
{
    Task MigrateAsync();
}

