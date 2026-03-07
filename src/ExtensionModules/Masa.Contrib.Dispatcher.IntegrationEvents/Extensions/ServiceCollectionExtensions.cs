// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

// ReSharper disable once CheckNamespace

using Masa.Contrib.Dispatcher.IntegrationEvents.BackgroundServices;
using Masa.Contrib.Dispatcher.IntegrationEvents.Internal;

namespace Masa.Contrib.Dispatcher.IntegrationEvents.Extensions;

public static class ServiceCollectionExtensions
{
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddIntegrationEventBus(
        this IServiceCollection services,
        Action<IntegrationEventOptions>? options = null)
        => services.AddIntegrationEventBus(MasaApp.GetAssemblies(), options);

    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddIntegrationEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options = null)
        => services.TryAddIntegrationEventBus(assemblies, options);

    public static IServiceCollection AddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        Action<IntegrationEventOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.AddIntegrationEventBus<TIntegrationEventLogService>(AppDomain.CurrentDomain.GetAssemblies(), options);

    public static IServiceCollection AddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options = null)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddIntegrationEventBus<TIntegrationEventLogService>(assemblies, options);

    internal static IServiceCollection TryAddIntegrationEventBus<TIntegrationEventLogService>(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options)
        where TIntegrationEventLogService : class, IIntegrationEventLogService
        => services.TryAddIntegrationEventBus(
            assemblies,
            options,
            services.TryAddScoped<IIntegrationEventLogService, TIntegrationEventLogService>);

    internal static IServiceCollection TryAddIntegrationEventBus(
        this IServiceCollection services,
        IEnumerable<Assembly> assemblies,
        Action<IntegrationEventOptions>? options,
        Action? action = null)
    {

#if (NET8_0_OR_GREATER)
        if (services.Any(service => service.IsKeyedService == false && service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;
#else
        if (services.Any(service => service.ImplementationType == typeof(IntegrationEventBusProvider)))
            return services;
#endif

        services.AddSingleton<IntegrationEventBusProvider>();

        MasaArgumentException.ThrowIfNull(assemblies);
        var dispatcherOptions = new IntegrationEventOptions(services, assemblies.Distinct().ToArray());
        options?.Invoke(dispatcherOptions);

        services.TryAddSingleton(typeof(IOptions<IntegrationEventOptions>),
            serviceProvider => Microsoft.Extensions.Options.Options.Create(dispatcherOptions));

        LocalQueueProcessor.SetLogger(services);
        services.AddScoped<IIntegrationEventBus>(serviceProvider => new IntegrationEventBus(
            serviceProvider,
            new Lazy<IEventBus?>(serviceProvider.GetService<IEventBus>),
            new Lazy<IPublisher>(serviceProvider.GetRequiredService<IPublisher>()),
            serviceProvider.GetService<IIntegrationEventLogService>(),
            serviceProvider.GetService<ILogger<IntegrationEventBus>>()
        ));
        action?.Invoke();

        if (services.Any(d => d.ServiceType == typeof(IIntegrationEventLogService)))
        {
            services.AddSingleton<IProcessor, RetryByDataProcessor>();
            services.AddSingleton<IProcessor, RetryByLocalQueueProcessor>();
            services.AddSingleton<IProcessor, SendByDataProcessor>();
            services.AddSingleton<IProcessor, DeletePublishedExpireEventProcessor>();
            services.AddSingleton<IProcessor, DeleteLocalQueueExpiresProcessor>();
        }
        else
        {
            var logger = services.BuildServiceProvider().GetService<ILogger<IntegrationEventBus>>();
            logger?.LogWarning("The local message table is not used correctly, it will cause integration events not to be sent normally");
        }

        services.TryAddSingleton<IProcessingServer, DefaultHostedService>();

        services.AddHostedService<IntegrationEventHostedService>();

        if (services.All(d => d.ServiceType != typeof(IPublisher)))
            throw new NotSupportedException($"{nameof(IPublisher)} has no implementing");

        MasaApp.TrySetServiceCollection(services);
        return services;
    }

    private sealed class IntegrationEventBusProvider
    {
    }
}
