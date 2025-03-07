#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class Category
{
    [MaxLength(36)]
    public required string Id { get; set; }
    public required string Name { get; set; }
}
