using System.Collections.Generic;
using Kafka.Communication.Models;
using MediatR;
using PaymentMicroservice.Domain.AggregateModel;

namespace PaymentMicroservice.Mediatr.Commands.CreatePaymentCommand
{
    public class CreatePaymentCommand: IRequest<PaymentEntity>
    {
        public string CorrelationId { get; set; }
        public string OrderId { get; set; }
        public string Username { get; set; }
        public string PaymentStatus { get; set; }
        public string RequestedTs { get; set; }
        public IList<Items> CostBreakdown { get; set; }
    }
    
    public class Items
    {
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public double CostPrice { get; set; }
    }
}