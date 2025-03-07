using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Data.Entities;

namespace Plutus.Infrastructure.Business.Obligors
{
    public class ChangeObligorFixedExpense(AppDbContext context)
    {
        public async Task<bool> Set(string obligorId, bool value)
        {
            return (await context.Obligors.Where(x => x.Id == obligorId)
                .UpdateFromQueryAsync(x => new Obligor
                {
                    DisplayName = x.DisplayName,
                    Id = x.Id,
                    Name = x.Name,
                    IsForFixedExpenses = value
                })) == 1;
        }
    }
}
