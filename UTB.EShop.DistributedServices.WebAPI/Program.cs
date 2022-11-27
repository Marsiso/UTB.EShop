using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using UTB.EShop.Application.DataTransferObjects.Carousel;
using UTB.EShop.Application.Interfaces.Models;
using UTB.EShop.Application.Interfaces.Repositories;
using UTB.EShop.Application.Paging;
using UTB.EShop.DistributedServices.WebAPI.Attributes;
using UTB.EShop.Infrastructure.Entities;
using UTB.EShop.Infrastructure.Repositories;
using UTB.EShop.DistributedServices.WebAPI.Extensions;
using UTB.EShop.Infrastructure.DbContexts;
using UTB.EShop.Infrastructure.Mappings;
using UTB.EShop.Infrastructure.Models;

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
    })
    .AddNewtonsoftJson()
    .AddXmlDataContractSerializerFormatters(); ;

builder.Logging
    .ClearProviders()
    .AddSerilog();

builder.Services
    .AddSqlServer<RepositoryContext>(builder.Configuration.GetConnectionString("Mssql"), opt => opt.MigrationsAssembly(typeof(Program).Assembly.FullName))
    .AddScoped<IRepository<CarouselItemEntity>, Repository<CarouselItemEntity>>()
    .AddScoped<IRepository<ImageFileEntity>, Repository<ImageFileEntity>>()
    .AddAutoMapper(typeof(CarouselItemProfile), typeof(ImageFileProfile))
    .AddScoped<ValidationFilterAttribute>()
    .AddScoped<ValidateCarouselItemExistsAttribute>()
    .AddScoped<IDataShaper<CarouselItemDto>, DataShaper<CarouselItemDto>>()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .ConfigureCors()
    .ConfigureIISIntegration();

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
    .UseRouting()
    .UseAuthorization()
    .UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

logger.Information("Project runtime behaviour have been configured.");

app.Run();
logger.Information("Project is running ...");