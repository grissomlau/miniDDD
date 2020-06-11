
using MiniDDD;
using System;

namespace DDD.Simple.Domain.Events
{
    public class OrderTotalAmountChanged : DomainEvent<Guid>
    {
        public decimal TotalAmount { get; set; }
        public OrderTotalAmountChanged(Guid aggregateRootKey, decimal totalAmount) : base(aggregateRootKey)
        {
            TotalAmount = totalAmount;
        }
    }
}
