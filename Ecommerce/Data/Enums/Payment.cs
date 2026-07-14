namespace Ecommerce.Data.Enums
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
