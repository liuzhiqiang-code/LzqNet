using System.Reflection;

public static class CustomMasaExtensions
{
    public static IServiceCollection AddCustomMasaRegistrationCaller(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var entryAssembly = Assembly.GetEntryAssembly()!;
        var callerAssemblyNames = builder.Configuration.GetSection("Masa:CallerAssemblyNames").Get<string[]>() ?? [];
        var loadedCallerAssemblies = new List<Assembly> { entryAssembly }
            .Concat(callerAssemblyNames.Select(Assembly.Load))
            .ToList();
        services.AddAutoRegistrationCaller(loadedCallerAssemblies);
         return services;
    }
}
