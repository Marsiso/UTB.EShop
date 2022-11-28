using AspNetCoreRateLimit;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Serilog;
using UTB.EShop.Application.DataShapers;
using UTB.EShop.Application.DataTransferObjects.Carousel;
using UTB.EShop.Application.Interfaces.Identity;
using UTB.EShop.Application.Interfaces.Models;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.DistributedServices.WebAPI.Attributes;
using UTB.EShop.Infrastructure.Entities;
using UTB.EShop.Infrastructure.Repositories;
using UTB.EShop.DistributedServices.WebAPI.Extensions;
using UTB.EShop.DistributedServices.WebAPI.Utility;
using UTB.EShop.Infrastructure.DbContexts;
using UTB.EShop.Infrastructure.Identity;
using UTB.EShop.Infrastructure.Mappings;

var builder = WebApplication.CreateBuilder(args);

var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(logger);

logger.Information("Project build has initiated.");

builder.Services
    .AddControllers(config =>
    {
        config.RespectBrowserAcceptHeader = true;
        config.ReturnHttpNotAcceptable = true;
        config.CacheProfiles.Add("120SecondsDuration", new CacheProfile { Duration = 120 });
    })
    .AddNewtonsoftJson();

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Services
    .AddCustomMediaTypes()
    .AddSqlServer<RepositoryContext>(builder.Configuration.GetConnectionString("Mssql"), opt => 
        opt.MigrationsAssembly(typeof(Program).Assembly.FullName))
    .AddScoped<IRepository<CarouselItemEntity>, Repository<CarouselItemEntity>>()
    .AddAutoMapper(typeof(CarouselItemProfile), typeof(ImageFileProfile), typeof(IdentityProfile))
    .AddScoped<ValidationFilterAttribute>()
    .AddScoped<ValidateCarouselItemExistsAttribute>()
    .AddScoped<IDataShaper<CarouselItemDto>, DataShaper<CarouselItemDto>>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureCors()
    .ConfigureIISIntegration()
    .AddScoped<ValidateMediaTypeAttribute>()
    .AddScoped<CarouselItemLinks>()
    .ConfigureVersioning()
    .ConfigureResponseCaching()
    .ConfigureHttpCacheHeaders()
    .AddMemoryCache()
    .ConfigureRateLimitingOptions()
    .AddHttpContextAccessor()
    .AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>()
    .AddAuthenticationWrapper()
    .ConfigureIdentity()
    .ConfigureJWT(builder.Configuration)
    .AddScoped<IAuthenticationManager, AuthenticationManager>();;

    
logger.Information("Services have been configured.");

var app = builder.Build();

logger.Information("Project build done.");

if (app.Environment.IsDevelopment())
    app
        .UseSwagger()
        .UseSwaggerUI();
else
    app.UseHsts();

app
    .UseHttpsRedirection()
    .UseStaticFiles()
    .UseCors("Default")
    .UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.All
    })
    .UseResponseCaching()
    .UseHttpCacheHeaders()
    .UseIpRateLimiting()
    .UseRouting()
    .UseAuthentication()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

logger.Information("Project runtime behaviour have been configured.");

app.Run();

logger.Information("Project is running ...");
logger.Dispose();