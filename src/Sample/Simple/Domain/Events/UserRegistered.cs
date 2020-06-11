
using MiniDDD;
using System;

namespace DDD.Simple.Domain.Events
{
    public class UserRegistered : DomainEvent<Guid>
    {
        public string Email { get; }
        public string Name { get; }
        public UserRegistered(Guid aggregateRootKey, string name, string email) : base(aggregateRootKey)
        {
            Name = name;
            Email = email;
        }
    }
}
