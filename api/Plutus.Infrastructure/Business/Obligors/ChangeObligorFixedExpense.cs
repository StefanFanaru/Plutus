namespace Plutus.Infrastructure.Business.Obligors;

public class ChangeObligorFixedExpense(IUserInfo userInfo, AppDbContext context)
{
    public async Task<bool> Set(string obligorId, bool value)
    {
        return await context.Obligors.Where(x => x.Id == obligorId)
            .ApplyUserFilter(userInfo.Id)
            .UpdateFromQueryAsync(x => new Obligor
            {
                DisplayName = x.DisplayName,
                UserId = x.UserId,
                Id = x.Id,
                Name = x.Name,
                IsForFixedExpenses = value
            }) == 1;
    }
}
