using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace UTB.EShop.DistributedServices.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services) =>
        services.AddCors(options => options.AddPolicy("Default", builder =>
        {
            builder.AllowAnyHeader();
            builder.AllowAnyMethod();
            builder.AllowAnyOrigin();
        }));

    public static IServiceCollection ConfigureIISIntegration(this IServiceCollection services) =>
        services.Configure<IISOptions>(options => { });
}