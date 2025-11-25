using Application.Common.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<ProductMapping>();

        }, Assembly.GetExecutingAssembly());

        return services;
    }
}
