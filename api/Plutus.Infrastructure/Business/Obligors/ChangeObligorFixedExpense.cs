namespace Plutus.Infrastructure.Business.Obligors;

public class ChangeObligorFixedExpense(IUserInfo userInfo, AppDbContext context)
{
    public async Task<bool> Set(string obligorDisplayName, bool value)
    {
        return await context.Obligors.Where(x => x.DisplayName == obligorDisplayName)
            .ApplyUserFilter(userInfo.Id)
            .UpdateFromQueryAsync(x => new Obligor
            {
                DisplayName = x.DisplayName,
                UserId = x.UserId,
                Id = x.Id,
                Name = x.Name,
                IsForFixedExpenses = value
            }) > 0;
    }
}
