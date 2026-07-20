namespace Domain.Enums
{
    public enum PaymentMethod
    {
        CashOnDelivery,
        CreditCard,
        Wallet
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Refunded,
        Cancelled
    }
}
