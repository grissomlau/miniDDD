﻿using MiniDDD;
using System;

namespace DDD.Simple.Domain.Events
{
    public class OrderPlaced : DomainEvent<Guid>
    {
        public decimal TotalAmount { get; set; }
        public OrderPlaced(decimal totalAmount) : base(Guid.NewGuid())
        {
            TotalAmount = totalAmount;
        }
    }
}
