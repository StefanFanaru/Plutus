#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class Requisition
{
    [MaxLength(36)]
    public required string Id { get; set; }
    [MaxLength(36)]
    public required string GoCardlessRequisitionId { get; set; }
    [MaxLength(36)]
    public User? User { get; set; }
    public required string UserId { get; set; }
    public required DateTime Created { get; set; }
    public bool IsConfirmed { get; set; }
}
