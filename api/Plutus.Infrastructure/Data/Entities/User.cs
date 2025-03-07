#nullable enable
using System.ComponentModel.DataAnnotations;


namespace Plutus.Infrastructure.Data.Entities;

public class User
{
    [MaxLength(36)]
    public required string Id { get; set; }
    [MaxLength(100)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string FirstName { get; set; }
    [MaxLength(100)]
    public required string LastName { get; set; }
    [MaxLength(100)]
    public required string Email { get; set; }
    [MaxLength(100)]
    public required string UserName { get; set; }
}
