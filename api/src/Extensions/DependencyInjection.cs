using TimeRegistration.Interfaces;
using TimeRegistration.Repositories;
using System.Reflection;


public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        
        //  Automatisk registrering for alle arkiver i den aktuelle assembly
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
