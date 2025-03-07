
namespace Plutus.Infrastructure.Abstractions;

public interface IUserInfo
{
    string Id { get; set; }
    string Name { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    string UserName { get; set; }
}
