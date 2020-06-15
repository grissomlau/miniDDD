using System;
using MiniDDD;
using SqlSugar;

namespace DDD.Simple.Model
{
    [SugarTable("Order")]
    public class Order : IEFModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Id { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
