using System;
using System.Linq;
using DDD.Simple.Domain;
using DDD.Simple.Domain.Events;
using Microsoft.EntityFrameworkCore;
using MiniDDD;
using MiniDDD.UnitOfWork;

namespace DDD.Simple.Repository.EF
{
    public class OrderRepository : Repository<Order, Guid>
    {
        readonly DbContext _dbContext;
        Model.Order _orderModel;
        public OrderRepository(IUnitOfWork unitOfWork)
        {
            _dbContext = unitOfWork.GetSqlClient<DbContext>();
        }
        public override Order Get(Guid key)
        {
            var userModel = _dbContext.Set<Model.Order>().AsQueryable().FirstOrDefault(x => x.Id == key);
            if (userModel != null)
            {
                return Order.Load(userModel.Id, userModel.TotalAmount, userModel.CreateTime);
            }
            return null;
        }

        public override void Save(Order aggreateRoot)
        {
            base.Save(aggreateRoot);
            _orderModel = null;
        }

        [InlineEventHandler]
        private void HandleOrderPlaced(OrderPlaced e)
        {
            var orderModel = GetOrderModel(e);
            orderModel.Id = (Guid)e.AggregateRootKey;
            orderModel.TotalAmount = e.TotalAmount;
            orderModel.CreateTime = e.Timestamp;
            _dbContext.Add(orderModel);
        }

        private Model.Order GetOrderModel(IDomainEvent<Guid> e)
        {
            if (_orderModel == null)
            {
                _orderModel = _dbContext.Set<Model.Order>().AsQueryable().FirstOrDefault(x => x.Id == (Guid)e.AggregateRootKey);
                _orderModel = _orderModel ?? new Model.Order();
            }
            return _orderModel;
        }
    }
}
