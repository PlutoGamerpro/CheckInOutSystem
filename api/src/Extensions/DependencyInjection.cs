using TimeRegistration.Interfaces;
using TimeRegistration.Repositories;
using System.Reflection;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        
        // Registro automático para todos os repositórios do assembly atual
        services.Scan(scan => scan
            .FromAssemblies(Assembly.GetExecutingAssembly())
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Repo")))
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }
}
