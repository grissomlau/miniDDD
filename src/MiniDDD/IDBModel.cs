using System;

namespace MiniDDD
{
    public interface IDbModel<out TKey> : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
    }
}
