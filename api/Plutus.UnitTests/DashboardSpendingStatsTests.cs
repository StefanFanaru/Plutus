using Plutus.Infrastructure.Business.Dashboard;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Data.Entities;

namespace Plutus.UnitTests;

public class DashboardSpendingStatsTests : IDisposable
{
    private AppDbContext _context;
    private readonly string _obligorId = Guid.NewGuid().ToString();
    private readonly string _categoryId = Guid.NewGuid().ToString();


    public DashboardSpendingStatsTests()
    {
        _context = TestHelpers.GetDbContext();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _context.Database.EnsureDeleted(); // Clear the database
        _context.Dispose(); // Dispose of the context
    }

    private void ResetDatabase()
    {
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
        _context = TestHelpers.GetDbContext();
    }

    private async Task SeedDatabase()
    {
        var days = 100;

        var obligor = new Obligor
        {
            Id = _obligorId,
            Name = "Test Obligor",
            DisplayName = "Test Obligor",
            IsForFixedExpenses = false
        };

        var category = new Category
        {
            Id = _categoryId,
            Name = "Test Category",
        };

        _context.Obligors.Add(obligor);
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var transactions = new List<Transaction>();

        for (int i = 0; i < days; i++)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                BookingDate = DateTime.UtcNow.AddDays(-i),
                Amount = 1 * i,
                ObligorId = obligor.Id,
                CategoryId = category.Id,
                IsCredit = true,
                Type = TransactionType.CardPayment
            };

            transactions.Add(transaction);
        }


        _context.Transactions.AddRange(transactions);
        await _context.SaveChangesAsync();
    }

    [Fact]
    public async Task Is_returning_correct_spend_per_day()
    {
        ResetDatabase();
        var dashboardSpendingStats = new DashboardSpendingStats(_context);

        await SeedDatabase();

        var transaction1 = new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            BookingDate = DateTime.UtcNow,
            Amount = 10.5m,
            ObligorId = _obligorId,
            CategoryId = _categoryId,
            IsCredit = true,
            Type = TransactionType.CardPayment
        };

        var transaction2 = new Transaction
        {
            Id = Guid.NewGuid().ToString(),
            BookingDate = DateTime.UtcNow,
            Amount = 11.5m,
            ObligorId = _obligorId,
            CategoryId = _categoryId,
            IsCredit = true,
            Type = TransactionType.CardPayment
        };
        _context.Transactions.Add(transaction1);
        _context.Transactions.Add(transaction2);
        await _context.SaveChangesAsync();

        var response = await dashboardSpendingStats.GetAsync();

        Assert.Equal(25, response.SpentPerDayLast25Days.Count);
        Assert.Equal(5, response.ProjectionNext5Days.Count);

        var first25Days = response.SpentPerDayLast25Days.First();

        Assert.Equal(DateTime.UtcNow.Date, first25Days.Date);
        Assert.Equal(22, first25Days.Amount);

        var projection = response.ProjectionNext5Days.First();
        Assert.Equal(DateTime.UtcNow.AddDays(1).Date, projection.Date.Date);
        Assert.Equal(15.5m, projection.Amount);
        Assert.Equal(487, response.TotalSpendLast30Days);
        Assert.Equal(-64, response.PercentageSpendingChange);
    }

    [Fact]
    public async Task Projection_and_median_is_correct()
    {
        var dashboardSpendingStats = new DashboardSpendingStats(_context);

        await SeedDatabase();

        async Task SeedTransaction(decimal amount, int days)
        {
            var transaction = new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                BookingDate = DateTime.UtcNow.AddDays(-days),
                Amount = amount,
                ObligorId = _obligorId,
                IsCredit = true,
                CategoryId = _categoryId,
                Type = TransactionType.CardPayment
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        await SeedTransaction(10, 0);
        await SeedTransaction(10, 1);
        await SeedTransaction(10, 2);
        await SeedTransaction(10, 3);
        await SeedTransaction(10, 4);

        var response = await dashboardSpendingStats.GetAsync();

        Assert.Equal(25, response.SpentPerDayLast25Days.Count);
        Assert.Equal(5, response.ProjectionNext5Days.Count);

        var first25Days = response.SpentPerDayLast25Days.First();

        Assert.Equal(DateTime.UtcNow.Date, first25Days.Date);
        Assert.Equal(10, first25Days.Amount);

        var projection = response.ProjectionNext5Days.First();
        Assert.Equal(DateTime.UtcNow.AddDays(1).Date, projection.Date.Date);
        Assert.Equal(14.5m, projection.Amount);
    }
}
