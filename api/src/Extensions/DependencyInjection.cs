using TimeRegistration.Interfaces;
using TimeRegistration.Repositories;
using System.Reflection;


public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        
        //  Automatic registration for all archives in the current assembly
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repo")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }
}
