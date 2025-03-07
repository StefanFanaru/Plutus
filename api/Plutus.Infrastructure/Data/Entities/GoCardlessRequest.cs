using System.ComponentModel.DataAnnotations;
using Plutus.Infrastructure.Enums;

namespace Plutus.Infrastructure.Data.Entities;

public class GoCardlessRequest
{
    [MaxLength(36)]
    public string Id { get; set; }
    public GoCardlessRequestType Type { get; set; }
    public DateTime MadeAt { get; set; }
}
