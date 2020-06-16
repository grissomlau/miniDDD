using EasyUnitOfWork;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDD.Simple.Model
{
    [Table("OrderItem")]
    public class OrderItem : IEFEntity
    {
        public Guid Id { get; set; }
    }
}
