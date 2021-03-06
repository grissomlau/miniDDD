﻿using DDD.Simple.Domain;
using MiniDDD;
using System;
using System.Data.Common;
using Dapper;
using DDD.Simple.Domain.Events;
using EasyUnitOfWork;

namespace Simple.Repository.Dapper
{
    public class OrderRepository : Repository<Order, Guid>
    {

        readonly DbConnection _dbConnection;
        DDD.Simple.Model.Order _order;
        public OrderRepository(IUnitOfWork unitOfWork)
        {
            _dbConnection = unitOfWork.GetUowWorker<DbConnection>();
        }

        public override Order Get(Guid key)
        {
            var order = _dbConnection.QuerySingleOrDefault<DDD.Simple.Model.Order>("select * from `order` where id = @id", new { id = key });
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
            _dbConnection.Execute("insert into `order`(Id, TotalAmount) values(@id, @totalamount)", new
            {
                id = e.AggregateRootKey,
                totalamount = e.TotalAmount
            });
        }

        private DDD.Simple.Model.Order GetOrderModel(IDomainEvent<Guid> e)
        {
            if (_order == null)
            {
                _order = _dbConnection.QuerySingleOrDefault<DDD.Simple.Model.Order>("Select * From `order` where id = @id", new { id = e.AggregateRootKey });
            }

            return _order ?? (_order = new DDD.Simple.Model.Order());
        }

    }
}
