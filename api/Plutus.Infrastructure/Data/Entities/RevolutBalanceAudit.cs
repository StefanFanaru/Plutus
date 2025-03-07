#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class RevolutBalanceAudit
{
    [MaxLength(36)]
    public required string Id { get; set; }

    [MaxLength(36)]
    public required string UserId { get; set; }
    public User? User { get; set; }

    [Required]
    public decimal Amount { get; set; }

    public DateTime RecordedAt { get; set; }
}
