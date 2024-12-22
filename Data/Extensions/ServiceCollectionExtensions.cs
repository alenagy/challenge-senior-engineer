using Data.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbContextWithInterceptors(
    this IServiceCollection services,
    Func<IServiceProvider, IEnumerable<IInterceptor>> interceptorFactory)
    {
        services.AddDbContext<TodoContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(
                serviceProvider.GetRequiredService<IConfiguration>().GetConnectionString("TodoContext"),
                b => b.MigrationsAssembly("TodoApi")); // Specify migrations assembly

            var interceptors = interceptorFactory(serviceProvider);
            foreach (var interceptor in interceptors)
            {
                options.AddInterceptors(interceptor);
            }
        });

        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<TodoContext>();
        SeedLastSyncProcess.Initialize(context);

        return services;
    }
}