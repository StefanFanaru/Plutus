using System.Globalization;
using static Plutus.Infrastructure.Services.GCGetData;

namespace Plutus.Infrastructure.Services;

public class GCInsertData(AppDbContext dbContext)
{
    public async Task InsertData(GoCardlessTransactionsReponse transactions, string userId)
    {
        var bookedTransactions = CleanTransactions(transactions);

        await InsertObligors(bookedTransactions, userId);
        var obligors = await dbContext.Obligors
            .Where(x => x.UserId == userId)
            .ToListAsync();

        var transactionBookedEntitties = bookedTransactions.Select(t =>
        {
            var obligorId = GetObligorId(t, obligors);
            return new Transaction
            {
                Id = t.TransactionId,
                UserId = userId,
                Type = t.GetTransactionType(),
                Amount = t.TransactionAmount.Amount,
                BookingDate = t.BookingDateTime,
                ObligorId = obligorId,
                CategoryId = GetCategoryId(obligorId, obligors),
                IsCredit = t.CreditorName != null
            };
        });

        var dbTransactions = await dbContext.Transactions
            .Where(x => x.UserId == userId)
            .ToListAsync();

        var filteredTransactions = new List<Transaction>();

        foreach (var transaction in transactionBookedEntitties)
        {
            if (dbTransactions.Any(x => x.Id == transaction.Id))
            {
                continue;
            }

            if (filteredTransactions.Any(x => x.Id == transaction.Id))
            {
                continue;
            }

            filteredTransactions.Add(transaction);
        }

        await dbContext.AddRangeAsync(filteredTransactions);
        await dbContext.SaveChangesAsync();
    }

    private static string GetObligorId(GCBooked bookedTransaction, List<Obligor> obligors)
    {
        if (bookedTransaction.CreditorName == null)
        {
            return obligors.Single(obligor => string.Equals(bookedTransaction.DebtorName, obligor.Name, StringComparison.OrdinalIgnoreCase)).Id;
        }
        return obligors.Single(obligor => string.Equals(bookedTransaction.CreditorName, obligor.Name, StringComparison.OrdinalIgnoreCase)).Id;
    }

    private static string GetCategoryId(string obligorId, List<Obligor> obligors)
    {
        var isFixed = obligors.Single(obligor => obligorId == obligor.Id).IsForFixedExpenses;
        return isFixed ? AppConstants.FixedCategoryId : AppConstants.UncategorizedCategoryId;
    }

    public static IEnumerable<GCBooked> CleanTransactions(GoCardlessTransactionsReponse transactions)
    {
        var ignoredTransactions = new[] { "TOPUP", "EXCHANGE", "FEE" };
        var bookedTransactions = transactions.Transactions.Booked
            .Where(x => !ignoredTransactions.Contains(x.ProprietaryBankTransactionCode))
            .Where(x => x.CreditorName != "STEFAN-EMANUEL FANARU" || !string.IsNullOrEmpty(x.DebtorName))
            .Where(x => x.CreditorName != "STEFAN EMANUEL FANARU" || !string.IsNullOrEmpty(x.DebtorName))
            .Where(x => x.CreditorName != "ȘTEFAN EMANUEL FÂNARU" || !string.IsNullOrEmpty(x.DebtorName))
            .Where(x => x.CreditorName != "STEFAN-EMANUEL FANARU" || x.ProprietaryBankTransactionCode != "TRANSFER")
            .Where(x => !string.IsNullOrEmpty(x.CreditorName) || !string.IsNullOrEmpty(x.DebtorName));
        return bookedTransactions;
    }

    private static string RemoveNamePrefix(string name)
    {
        var removablePrefixes = new[] { "Payu", "Ep", "Bkg" };
        foreach (var prefix in removablePrefixes)
        {
            if (name.StartsWith(prefix) || name.StartsWith(prefix, StringComparison.CurrentCultureIgnoreCase))
            {
                return name[(prefix.Length + 1)..].Trim();
            }
        }

        return name;
    }

    public async Task InsertObligors(IEnumerable<GCBooked> bookedTransactions, string userId)
    {
        var fixedExpensesObligorsPath = Path.Combine(Directory.GetCurrentDirectory(), "fixed_expenses_obligors.json");
        var fixedExpensesObligorsJson = await File.ReadAllTextAsync(fixedExpensesObligorsPath);
        var fixedExpensesObligors = JsonConvert.DeserializeObject<List<string>>(fixedExpensesObligorsJson);

        var obligors = bookedTransactions
            .Select(t => t.CreditorName)
            .Union(bookedTransactions.Select(t => t.DebtorName))
            .Where(x => !string.IsNullOrEmpty(x))
            .Distinct().Select(x => new Obligor
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Name = x,
                DisplayName = FormatName(x),
                IsForFixedExpenses = fixedExpensesObligors.Contains(x)
            });

        var dbObligors = await dbContext.Obligors
            .Where(x => x.UserId == userId)
            .ToListAsync();

        var filteredObligors = new List<Obligor>();

        foreach (var obligor in obligors)
        {
            if (filteredObligors.Any(x => string.Equals(x.Name, obligor.Name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            if (dbObligors.Any(x => string.Equals(x.Name, obligor.Name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            filteredObligors.Add(obligor);
        }

        await dbContext.AddRangeAsync(filteredObligors);
        await dbContext.SaveChangesAsync();
        return;

        static string FormatName(string name)
        {
            // if the name is full uppercase, convert it to title case
            if (name == name.ToUpper())
            {
                return RemoveNamePrefix(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower()));
            }

            return RemoveNamePrefix(name);
        }
    }
}
