#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class Obligor
{
    [MaxLength(36)]
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public bool IsForFixedExpenses { get; set; }
}

