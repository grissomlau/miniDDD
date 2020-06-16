using System;
using DDD.Simple.Domain;
using DDD.Simple.Domain.Events;
using EasyUnitOfWork;
using MiniDDD;
using SqlSugar;

namespace DDD.Simple.Repository.SqlSugar
{
    public class OrderRepository : Repository<Order, Guid>
    {
        readonly SqlSugarClient _sqlClient;
        Model.Order _order;

        public OrderRepository(IUnitOfWork unitOfWork)
        {
            _sqlClient = unitOfWork.GetUowWorker<SqlSugarClient>();
        }

        public override Order Get(Guid key)
        {
            var order = _sqlClient.Queryable<Model.Order>().Where(x => x.Id == key).Single();
            return Order.Load(order.Id, order.TotalAmount, order.CreateTime);

        }

        public override void Save(Order aggreateRoot)
        {
            base.Save(aggreateRoot);
            _order = null;
        }

        [InlineEventHandler]
        private void HandleOrderPlaced(OrderPlaced e)
        {
            var orderModel = GetOrderModel(e);
            orderModel.Id = e.Id;
            orderModel.TotalAmount = e.TotalAmount;
            orderModel.CreateTime = e.Timestamp;
            _sqlClient.Insertable(orderModel).ExecuteCommand();
        }

        private Model.Order GetOrderModel(IDomainEvent<Guid> e)
        {
            if (_order == null)
            {
                _order = _sqlClient.Queryable<Model.Order>().Where(x => x.Id == (Guid)e.AggregateRootKey).Single();
            }

            return _order ?? (_order = new Model.Order());
        }

    }
}
