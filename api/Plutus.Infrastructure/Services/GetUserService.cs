using Newtonsoft.Json.Converters;

namespace Plutus.Infrastructure.Services;

public class GetUserService(IUserInfo userInfo, AppDbContext dbContext)
{
    public async Task<Response> GetAsync()
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(user => user.Id == userInfo.Id);
        if (user == null)
        {
            dbContext.Users.Add(new User
            {
                Id = userInfo.Id,
                Email = userInfo.Email,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                UserName = userInfo.UserName,
                Name = userInfo.Name,
                Status = UserStatus.New
            });

            await dbContext.SaveChangesAsync();
        }

        return new Response
        {
            Id = userInfo.Id,
            Email = userInfo.Email,
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            UserName = userInfo.UserName,
            Name = userInfo.Name,
            Status = user?.Status ?? UserStatus.New
        };
    }
    public class Response
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public UserStatus Status { get; set; }

    }
}
