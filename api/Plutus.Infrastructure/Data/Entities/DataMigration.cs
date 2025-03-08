#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class DataMigration
{
    [MaxLength(36)]
    public required string Id { get; set; }
    [MaxLength(100)]
    public required string Name { get; set; }
    [MaxLength(100)]
    public required string Type { get; set; }
    public DateTime InsertTime { get; set; }
}
