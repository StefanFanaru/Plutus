namespace Plutus.Infrastructure.Services;

public class ValidateUser(IUserInfo userInfo, AppDbContext dbContext)
{
    public async Task<bool> IsValidUser()
    {
        var exists = dbContext.Users.Any(user => user.Id == userInfo.Id);
        if (!exists)
        {
            dbContext.Users.Add(new User
            {
                Id = userInfo.Id,
                Email = userInfo.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                UserName = userInfo.UserName,
                Name = userInfo.Name
            });

            await dbContext.SaveChangesAsync();
        }


        return true;
    }
}
