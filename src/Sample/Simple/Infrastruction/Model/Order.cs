using System;
using EasyUnitOfWork;
using SqlSugar;

namespace DDD.Simple.Model
{
    [SugarTable("Order")]
    public class Order : IEFEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
