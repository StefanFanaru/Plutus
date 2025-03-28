using static Plutus.Infrastructure.Services.GCGetData;

namespace Plutus.Infrastructure.Services;

public class InsertSampleData(AppDbContext dbContext, GCInsertData insertDataService)
{
    private async Task InsertBalances()
    {

        // insert a RevolutBalanceAudit for the last 40 days with a balance that started at 3000 and decreased by random 50-80 each day
        var random = new Random();
        var balance = 3000m;
        for (var i = 40; i >= 0; i--)
        {
            balance -= random.Next(20, 100);
            await dbContext.BalanceAudits.AddAsync(new RevolutBalanceAudit
            {
                Id = Guid.NewGuid().ToString(),
                UserId = "b6ca41ec-c3d0-44be-aa13-4322d370f625",
                Amount = balance,
                RecordedAt = DateTime.UtcNow.AddDays(-i)
            });
        }
        await dbContext.SaveChangesAsync();

    }

    private async Task InsertCategories()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "categories.json");
        var jsonData = await File.ReadAllTextAsync(filePath);

        var categories = JsonConvert.DeserializeObject<List<Category>>(jsonData);

        await dbContext.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();
    }

    private async Task InsertTransactions()
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "sampledata.json");
        var jsonData = await File.ReadAllTextAsync(filePath);

        var transactions = JsonConvert.DeserializeObject<GoCardlessTransactionsReponse>(jsonData);

        await insertDataService.InsertData(transactions, "b6ca41ec-c3d0-44be-aa13-4322d370f625");
    }

    private async Task AssignRandomCategoriesToTransactions()
    {

        Console.WriteLine("Assigning random categories to transactions");
        var categories = await dbContext.Categories.ToListAsync();
        var transactions = await dbContext.Transactions.ToListAsync();
        var random = new Random();

        foreach (var transaction in transactions)
        {
            var randomCategory = categories[random.Next(0, categories.Count)];
            transaction.CategoryId = randomCategory.Id;
        }

        await dbContext.SaveChangesAsync();
    }


    public async Task InsertSampleDataAsync()
    {
        if (!await dbContext.Categories.AnyAsync(x => x.Name != "Uncategorized"))
        {
            await InsertCategories();
        }

        if (!await dbContext.Transactions.AnyAsync())
        {
            await InsertTransactions();

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                await AssignRandomCategoriesToTransactions();
            }
        }

        if (!await dbContext.BalanceAudits.AnyAsync())
        {
            await InsertBalances();
        }
    }
}
