namespace Plutus.Infrastructure.Services;

public class SaveSelectedAccount(IUserInfo userInfo, AppDbContext dbContext, GCDataCollection dataCollection)
{
    public async Task<bool> SelectAccountAsynct(string accountId)
    {
        var accountIdUpdated = await dbContext.Users.Where(x => x.Id == userInfo.Id)
            .UpdateFromQueryAsync(x => new User
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Name = x.Name,
                Status = UserStatus.RevolutConfirmed,
                RevolutAccountId = accountId
            }) == 1;

        if (!accountIdUpdated)
        {
            return false;
        }

        await dataCollection.CollectData(userInfo.Id, 730);

        return true;
    }
}

