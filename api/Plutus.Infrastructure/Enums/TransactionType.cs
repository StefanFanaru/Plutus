#nullable enable
namespace Plutus.Infrastructure.Data.Entities;

public enum TransactionType
{
    Transfer,
    CardPayment,
    CardRefund,
    RevolutPayment
}
