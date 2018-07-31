using Autofac;

namespace MiniDDD.UnitOfWork.EF
{
    public class DefaultModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().AsImplementedInterfaces().InstancePerLifetimeScope();
            builder.RegisterType<DefaultDbContext>().SingleInstance();
        }
    }
}
