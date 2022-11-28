using System.Text;
using AspNetCoreRateLimit;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UTB.EShop.Infrastructure.DbContexts;
using UTB.EShop.Infrastructure.Identity;

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
    
    public static IServiceCollection AddCustomMediaTypes(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(config =>
        {
            var newtonsoftJsonOutputFormatter = config.OutputFormatters
                .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();
            if (newtonsoftJsonOutputFormatter != null)
            {
                newtonsoftJsonOutputFormatter
                    .SupportedMediaTypes
                    .Add("application/vnd.hateoas+json");
                newtonsoftJsonOutputFormatter
                    .SupportedMediaTypes
                    .Add("application/vnd.apiroot+json");
            }
        });

        return services;
    }
    
    public static IServiceCollection ConfigureVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(opt =>
        {
            opt.ReportApiVersions = true;
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
        });

        return services;
    }
    
    public static IServiceCollection ConfigureResponseCaching(this IServiceCollection services) => 
        services.AddResponseCaching();
    
    public static IServiceCollection ConfigureHttpCacheHeaders(this IServiceCollection services) => 
        services.AddHttpCacheHeaders(
            (expirationOpt) =>
            {
                expirationOpt.MaxAge = 65;
                expirationOpt.CacheLocation = CacheLocation.Private;
            },
            (validationOpt) =>
            {
                validationOpt.MustRevalidate = true;
            });

    public static IServiceCollection ConfigureRateLimitingOptions(this IServiceCollection services)
    {
        var rateLimitRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit= 30,
                Period = "5m"
            }
        };
        
        services.Configure<IpRateLimitOptions>(opt =>
        {
            opt.GeneralRules = rateLimitRules;
        });
        
        services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

        return services;
    }
    
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services)
    {
        var builder = services.AddIdentityCore<User>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 10;
            options.User.RequireUniqueEmail = true;
        });
        
        builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
        builder
            .AddEntityFrameworkStores<RepositoryContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddAuthenticationWrapper(this IServiceCollection services)
    {
        services.AddAuthentication();
        return services;
    }
    
    public static IServiceCollection ConfigureJWT(this IServiceCollection services, IConfiguration 
        configuration)
    {
        var jwtSettings = configuration.GetSection("JwtSettings");
        //var secretKey = Environment.GetEnvironmentVariable("JWTSECRET");
        
        services
            .AddAuthentication(opt => {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("serverSecret").Value))
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)
                };
            });

        return services;
    }

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UTB EShop WebAPI", 
                Version = "v1",
                Description = "EShop WebAPI by UTB",
                TermsOfService = new Uri("https://example.com/terms"),
                Contact = new OpenApiContact
                {
                    Name = "Marek Olsak",
                    Email = "m_olsak@utb.czm",
                    Url = new Uri("https://twitter.com/m_olsak"),
                },
                License = new OpenApiLicense
                {
                    Name = "EShop WebAPI LICX",
                    Url = new Uri("https://example.com/license"),
                }
            });
            
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Place to add JWT with Bearer",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer",
                    },
                    new List<string>()
                }
            });
        });

        return services;
    }

}