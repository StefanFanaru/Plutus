
using Plutus.Infrastructure.Abstractions;

namespace Plutus.UnitTests.TestDtos
{
    public class TestUserInfo : IUserInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
    }
}
