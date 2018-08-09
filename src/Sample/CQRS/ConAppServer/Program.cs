using System;
using Autofac;
using Jimu.Server;

namespace DDD.CQRS.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var containerBuilder = new ContainerBuilder();
            var builder = new ServiceHostServerBuilder(containerBuilder)
                .LoadServices(new[] { "CQRS.IServices", "CQRS.Services" })
                .UseMasstransit(new MassTransitOptions
                {
                    HostAddress = new Uri("rabbitmq://localhost/"),
                    Username = "guest",
                    Password = "guest",
                    QueueName = "Jimu_test",
                    SendEndPointUri = new Uri("rabbitmq://localhost/Jimu_test")
                })
                .UseDotNettyForTransfer("127.0.0.1", 8006, server => { })
                .UseConsulForDiscovery("127.0.0.1", 8500, "JimuService", "127.0.0.1:8006")
                ;
            using (var host = builder.Build())
            {
                host.Run();
                Console.WriteLine("Server start successful.");
                Console.ReadLine();
            }
        }
    }
}
