
using MiniDDD;
using System;

namespace DDD.CQRS.Domain.Events
{
    public class UserCreated : DomainEvent<Guid>
    {
        public UserCreated(Guid aggregateRootKey, string name, string email) : base(aggregateRootKey)
        {
            Name = name;
            Email = email;
        }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
