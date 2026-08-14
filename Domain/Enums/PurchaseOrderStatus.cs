namespace Domain.Enums;

public enum PurchaseOrderStatus
{
    Draft = 1,
    PendingApproval = 2,
    Approved = 3,
    PartiallyReceived = 4,
    Completed = 5,
    Cancelled = 6
}
