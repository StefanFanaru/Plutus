namespace Plutus.Infrastructure.Business.Transactions;

public class TransactionListItem
{
    public string Id { get; set; }
    public DateTime BookedAt { get; set; }
    public decimal Amount { get; set; }
    public TransactionType? Type { get; set; }
    public string ObligorId { get; set; }
    public string ObligorName { get; set; }
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public bool IsExcluded { get; set; }
    public bool IsSplitItem { get; set; }
    public bool IsFixedExpense { get; set; }
}

