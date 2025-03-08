#nullable enable
using System.ComponentModel.DataAnnotations;

namespace Plutus.Infrastructure.Data.Entities;

public class GoCardlessRequest
{
    [MaxLength(36)]
    public required string Id { get; set; }
    [MaxLength(36)]
    public required string UserId { get; set; }
    public User? User { get; set; }
    public GoCardlessRequestType Type { get; set; }
    public DateTime MadeAt { get; set; }
}
