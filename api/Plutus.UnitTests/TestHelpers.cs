using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data;

namespace Plutus.UnitTests;

public static class TestHelpers
{

    public static AppDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var dbContext = new AppDbContext(options);

        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}
