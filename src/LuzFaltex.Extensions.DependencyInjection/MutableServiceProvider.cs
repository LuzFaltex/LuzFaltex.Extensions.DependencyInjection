using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.ServiceLookup;

namespace LuzFaltex.Extensions.DependencyInjection;

/// <summary>
/// The mutable IServiceProvider.
/// </summary>
// Made from the default ServiceProvider
// (with changes to allow the adding/removing of services after the provider has been created.
public class MutableServiceProvider : IServiceProvider, IDisposable, IAsyncDisposable
{
    internal const string RequiresDynamicCodeMessage = "Using Microsoft.Extensions.DependencyInjection requires generating code dynamically at runtime. For example, when using enumerable and generic ValueType services.";

    private readonly CallSiteValidator? _callSiteValidator;

    private readonly Func<Type, Func<ServiceProviderEngineScope, object?>> _createServiceAccessor;
    private readonly ServiceProviderOptions? _options;

    // Internal for testing
    internal ServiceProviderEngine _engine;

    private bool _disposed;

    private ConcurrentDictionary<Type, Func<ServiceProviderEngineScope, object?>> _realizedServices;

    internal CallSiteFactory CallSiteFactory { get; }

    internal ServiceProviderEngineScope Root { get; }

    internal static bool VerifyOpenGenericServiceTrimmability { get; } =
        AppContext.TryGetSwitch("Microsoft.Extensions.DependencyInjection.VerifyOpenGenericServiceTrimmability", out var verifyOpenGenerics) ? verifyOpenGenerics : false;

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableServiceProvider"/> class.
    /// </summary>
    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    public MutableServiceProvider()
    {
        // note that Root needs to be set before calling GetEngine(), because the engine may need to access Root
        Root = new ServiceProviderEngineScope(this, isRootScope: true);
        _engine = GetEngine();
        _createServiceAccessor = CreateServiceAccessor;
        _realizedServices = new ConcurrentDictionary<Type, Func<ServiceProviderEngineScope, object?>>();
        CallSiteFactory = new CallSiteFactory();

        // The list of built in services that aren't part of the list of service descriptors
        // keep this in sync with CallSiteFactory.IsService
        CallSiteFactory.Add(typeof(IServiceProvider), new ServiceProviderCallSite());
        CallSiteFactory.Add(typeof(IServiceScopeFactory), new ConstantCallSite(typeof(IServiceScopeFactory), Root));
        CallSiteFactory.Add(typeof(IServiceProviderIsService), new ConstantCallSite(typeof(IServiceProviderIsService), CallSiteFactory));
    }

    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    internal MutableServiceProvider(IServiceCollection services, ServiceProviderOptions options)
        : this()
    {
        _options = options;
        AddServices("Default", services);
        if (_options.ValidateScopes)
        {
            _callSiteValidator = new CallSiteValidator();
        }
    }

    /// <summary>
    /// Adds new services to the mutable service provider under a name in which makes it easy to
    /// Remove services in bulk selectively.
    /// </summary>
    /// <param name="name">The name for which services are to be registered under.</param>
    /// <param name="services">The services to add.</param>
    /// <exception cref="AggregateException">When the added services cannot be constructed,</exception>
    public void AddServices(string name, IServiceCollection services)
    {
        CallSiteFactory.AddServiceCollection(name, services);
        if (CallSiteFactory.IsDefaultServiceCollectionAdded())
        {
            DependencyInjectionEventSource.Log.ServiceProviderBuilt(this);
        }

        if (_options?.ValidateOnBuild == true)
        {
            List<Exception>? exceptions = null;
            foreach (var serviceDescriptor in services)
            {
                try
                {
                    ValidateService(serviceDescriptor);
                }
                catch (Exception e)
                {
                    exceptions ??= new List<Exception>();
                    exceptions.Add(e);
                }
            }

            if (exceptions != null)
            {
                throw new AggregateException("Some services are not able to be constructed", exceptions.ToArray());
            }
        }
    }

    /// <summary>
    /// Removes a services in bulk that are stored under a specific name.
    /// </summary>
    /// <param name="name">The name for which services were registered under.</param>
    public void RemoveServices(string name)
    {
        CallSiteFactory.RemoveServiceCollection(name);
    }

    /// <summary>
    /// Gets the service object of the specified type.
    /// </summary>
    /// <param name="serviceType">The type of the service to get.</param>
    /// <returns>The service that was produced.</returns>
    public object? GetService(Type serviceType) => GetService(serviceType, Root);

    internal bool IsDisposed() => _disposed;

    /// <inheritdoc />
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        DisposeCore();
        Root.Dispose();
    }

    /// <inheritdoc/>
    public ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        DisposeCore();
        return Root.DisposeAsync();
    }

    private void DisposeCore()
    {
        _disposed = true;
        DependencyInjectionEventSource.Log.ServiceProviderDisposed(this);
    }

    private void OnCreate(ServiceCallSite callSite)
    {
        _callSiteValidator?.ValidateCallSite(callSite);
    }

    private void OnResolve(Type serviceType, IServiceScope scope)
    {
        _callSiteValidator?.ValidateResolution(serviceType, scope, Root);
    }

    internal object? GetService(Type serviceType, ServiceProviderEngineScope serviceProviderEngineScope)
    {
        if (_disposed)
        {
            ThrowHelper.ThrowObjectDisposedException();
        }

        var realizedService = _realizedServices.GetOrAdd(serviceType, _createServiceAccessor);
        OnResolve(serviceType, serviceProviderEngineScope);
        DependencyInjectionEventSource.Log.ServiceResolved(this, serviceType);
        var result = realizedService.Invoke(serviceProviderEngineScope);
        System.Diagnostics.Debug.Assert(result is null || CallSiteFactory.IsService(serviceType));
        return result;
    }

    private void ValidateService(ServiceDescriptor descriptor)
    {
        if (descriptor.ServiceType.IsGenericType && !descriptor.ServiceType.IsConstructedGenericType)
        {
            return;
        }

        try
        {
            var callSite = CallSiteFactory.GetCallSite(descriptor, new CallSiteChain());
            if (callSite != null)
            {
                OnCreate(callSite);
            }
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error while validating the service descriptor '{descriptor}': {e.Message}", e);
        }
    }

    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    private Func<ServiceProviderEngineScope, object?> CreateServiceAccessor(Type serviceType)
    {
        var callSite = CallSiteFactory.GetCallSite(serviceType, new CallSiteChain());
        if (callSite != null)
        {
            DependencyInjectionEventSource.Log.CallSiteBuilt(this, serviceType, callSite);
            OnCreate(callSite);

            // Optimize singleton case
            if (callSite.Cache.Location == CallSiteResultCacheLocation.Root)
            {
                var value = CallSiteRuntimeResolver.Instance.Resolve(callSite, Root);
                return scope => value;
            }

            return _engine.RealizeService(callSite);
        }

        return _ => null;
    }

    internal void ReplaceServiceAccessor(ServiceCallSite callSite, Func<ServiceProviderEngineScope, object?> accessor)
    {
        _realizedServices[callSite.ServiceType] = accessor;
    }

    internal IServiceScope CreateScope()
    {
        if (_disposed)
        {
            ThrowHelper.ThrowObjectDisposedException();
        }

        return new ServiceProviderEngineScope(this, isRootScope: false);
    }

    [RequiresDynamicCode(RequiresDynamicCodeMessage)]
    private ServiceProviderEngine GetEngine()
    {
        ServiceProviderEngine engine;

#if NETFRAMEWORK || NETSTANDARD2_0
        engine = new DynamicServiceProviderEngine(this);
#else
        if (RuntimeFeature.IsDynamicCodeCompiled)
        {
            engine = new DynamicServiceProviderEngine(this);
        }
        else
        {
            // Don't try to compile Expressions/IL if they are going to get interpreted
            engine = RuntimeServiceProviderEngine.Instance;
        }
#endif
        return engine;
    }
}
