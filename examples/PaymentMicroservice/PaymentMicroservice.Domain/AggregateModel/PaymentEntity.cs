using System;

namespace PaymentMicroservice.Domain.AggregateModel
{
    public class PaymentEntity
    {
        public string PaymentId { get; set; }
        public string OrderId { get; set; }
        public double AmountPayable { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string Username { get; set; }
        public DateTime CreatedTs { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Paid,
        Rejected
    }
}