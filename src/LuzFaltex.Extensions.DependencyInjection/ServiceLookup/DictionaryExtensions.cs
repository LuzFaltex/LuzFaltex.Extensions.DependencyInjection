using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection.ServiceLookup;

internal static class DictionaryExtensions
{
    public static ServiceCount CountServices(this Dictionary<string, IServiceCollection> serviceCollections)
    {
        return new ServiceCount(serviceCollections.Values);
    }

    internal class ServiceCount
    {
        private readonly List<ServiceDescriptor> _serviceCollection;
        public ServiceCount(Dictionary<string, IServiceCollection>.ValueCollection serviceCollections)
        {
            // flatten to a single service collection.
            _serviceCollection = (
                from serviceCollection in serviceCollections
                from descriptors in serviceCollection
                select descriptors).ToList();
            Count = _serviceCollection.Count();
        }

        public int Count { get; set; }

        public ServiceDescriptor this[int key]
        {
            get => _serviceCollection[key];
            set => _serviceCollection[key] = value;
        }
    }
}
