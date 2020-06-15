using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Concurrent;

namespace MiniDDD.UnitOfWork.EF
{
    public sealed class EFProviderContainer
    {

        public static EFProviderContainer Instance;


        EFProviderContainer() { }
        static EFProviderContainer()
        {
            Instance = new EFProviderContainer();
        }

        ConcurrentDictionary<string, IEFProvider> _providers = new ConcurrentDictionary<string, IEFProvider>();

        public IEFProvider GetFactory(string providerName)
        {
            if (_providers.ContainsKey(providerName))
            {
                return _providers[providerName];
            }
            return null;
        }

        public void RegisterFactory(string providerName, IEFProvider factory)
        {
            _providers[providerName] = factory;
        }
    }
}
