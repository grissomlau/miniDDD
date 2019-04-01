using System;
using Autofac;
using DDD.Simple.Domain;
using MiniDDD;

namespace Simple.Repository.Dapper
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
