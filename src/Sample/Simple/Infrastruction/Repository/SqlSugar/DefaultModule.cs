using Autofac;
using DDD.Simple.Domain;
using MiniDDD;
using System;

namespace DDD.Simple.Repository.SqlSugar
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserRepository>().As<IRepository<User, Guid>>().InstancePerLifetimeScope();
            builder.RegisterType<OrderRepository>().As<IRepository<Order, Guid>>().InstancePerLifetimeScope();
        }

    }
}
