#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class Transaction
{
    [MaxLength(36)]
    public required string Id { get; set; }

    [Required]
    [MaxLength(36)]
    public decimal Amount { get; set; }

    public DateTime BookingDate { get; set; }

    [Required]
    public TransactionType? Type { get; set; }

    [MaxLength(36)]
    public string? ObligorId { get; set; }

    public Obligor? Obligor { get; set; }

    public bool IsCredit { get; set; }

    [MaxLength(36)]
    public required string CategoryId { get; set; }

    public Category? Category { get; set; }

    public bool IsExcluded { get; set; }

    public bool IsSplit { get; set; }

    [MaxLength(36)]
    public string? OriginalTransactionId { get; set; }

    public Transaction? OriginalTransaction { get; set; }
}
