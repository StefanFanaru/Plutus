// Root myDeserializedclass GC= JsonConvert.DeserializeObject<Root>(myJsonResponse);

namespace Plutus.Infrastructure.Dtos;

public class GCBooked
{
    [JsonProperty("transactionId")]
    public string TransactionId { get; set; }

    [JsonProperty("creditorName")]
    public string CreditorName { get; set; }

    [JsonProperty("debtorName")]
    public string DebtorName { get; set; }

    [JsonProperty("transactionAmount")]
    public GCTransactionAmount TransactionAmount { get; set; }

    [JsonProperty("bookingDateTime")]
    public DateTime BookingDateTime { get; set; }

    [JsonProperty("valueDateTime")]
    public DateTime ValueDateTime { get; set; }

    [JsonProperty("proprietaryBankTransactionCode")]
    public string ProprietaryBankTransactionCode { get; set; }

    [JsonProperty("remittanceInformationUnstructuredArray")]
    public List<string> RemittanceInformationUnstructuredArray { get; set; }


    public TransactionType GetTransactionType()
    {
        return ProprietaryBankTransactionCode switch
        {
            "TRANSFER" => TransactionType.Transfer,
            "CARD_PAYMENT" => TransactionType.CardPayment,
            "CARD_REFUND" => TransactionType.CardRefund,
            "REV_PAYMENT" => TransactionType.RevolutPayment,
            _ => throw new Exception($"Unknown transaction type {ProprietaryBankTransactionCode}"),
        };
    }
}

public class GCPending
{
    [JsonProperty("transactionAmount")]
    public GCTransactionAmount TransactionAmount { get; set; }

    [JsonProperty("bookingDateTime")]
    public DateTime BookingDateTime { get; set; }

    [JsonProperty("remittanceInformationUnstructuredArray")]
    public List<string> RemittanceInformationUnstructuredArray { get; set; }
}

public class GCTransactionAmount
{
    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}

public class GCTransactions
{
    [JsonProperty("booked")]
    public List<GCBooked> Booked { get; set; }

    [JsonProperty("pending")]
    public List<GCPending> Pending { get; set; }
}