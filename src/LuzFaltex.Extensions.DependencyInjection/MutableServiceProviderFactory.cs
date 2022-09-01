using System;
using Microsoft.Extensions.DependencyInjection;

namespace LuzFaltex.Extensions.DependencyInjection;

/// <summary>
/// A service provider factory for a mutable service provider.
/// Use in place of the default service provider factory.
/// </summary>
public class MutableServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
{
    private readonly ServiceProviderOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableServiceProviderFactory"/> class
    /// with default options.
    /// </summary>
    public MutableServiceProviderFactory()
        : this(new ServiceProviderOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableServiceProviderFactory"/> class
    /// with the specified <paramref name="options"/>.
    /// </summary>
    /// <param name="options">The options to use for this instance.</param>
    public MutableServiceProviderFactory(ServiceProviderOptions options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    /// <inheritdoc />
    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return new MutableServiceProvider(containerBuilder, _options);
    }
}
