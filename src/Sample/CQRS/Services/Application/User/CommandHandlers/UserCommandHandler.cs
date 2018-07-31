using System;
using System.Threading.Tasks;
using DDD.CQRS.Domain.Events;
using DDD.CQRS.IServices.Commands;
using MassTransit;

namespace DDD.CQRS.Services.Application.User.CommandHandlers
{
    public class UserCommandHandler :
        IConsumer<CreateUser>,
        IConsumer<UpdateUser>
    {

        public Task Consume(ConsumeContext<CreateUser> context)
        {
            Console.WriteLine("consume createuser");
            var user = new DDD.CQRS.Domain.User(context.Message.Id, context.Message.Name, context.Message.Email);
            foreach (var e in user.UncommittedEvents)
            {
                context.Publish(e, e.GetType());
            }
            user.Purge();
            return Task.CompletedTask;
        }

        public Task Consume(ConsumeContext<UpdateUser> context)
        {
            Console.WriteLine("consume updateuser");
            //var user = new Domain.User();
            return Task.CompletedTask;
        }
    }
}
