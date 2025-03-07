#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class Obligor
{
    [MaxLength(36)]
    public required string Id { get; set; }
    [MaxLength(36)]
    public required string UserId { get; set; }
    public User? User { get; set; }
    [MaxLength(100)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string DisplayName { get; set; }
    public bool IsForFixedExpenses { get; set; }
}

