using Domain.Enums;

namespace Domain.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;


        public PaymentMethod Method { get; set; }

        public PaymentStatus Status { get; set; }


        // Gateway information
        public string? TransactionId { get; set; }

        public string? GatewayResponse { get; set; }


        public decimal Amount { get; set; }


        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}