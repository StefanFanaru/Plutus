using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Plutus.API.Asp;
using Plutus.Infrastructure.Business.Categories;
using Plutus.Infrastructure.Business.Dashboard;
using Plutus.Infrastructure.Business.Obligors;
using Plutus.Infrastructure.Business.Transactions;
using Plutus.Infrastructure.Common;
using Plutus.Infrastructure.Data;
using Serilog;
using Serilog.Events;

namespace Plutus.API;

public static class AppConfiguration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddAllDataMigrations();

        services.AddSingleton<IAuthorizationHandler, PlutusUserHandler>();

        services.AddScoped<DataMigrator>();
        services.AddScoped<GCAuth>();
        services.AddScoped<GCGetData>();
        services.AddScoped<GCDataCollection>();
        services.AddScoped<GCInsertData>();
        services.AddScoped<InsertSampleData>();
        services.AddScoped<ListObligors>();
        services.AddScoped<DashboardStats>();
        services.AddScoped<DashboardSpendingStats>();
        services.AddScoped<ListTransactions>();
        services.AddScoped<ListCategories>();
        services.AddScoped<ExcludeTransaction>();
        services.AddScoped<ChangeTransactionCategory>();
        services.AddScoped<SplitTransaction>();
        services.AddScoped<UnsplitTransaction>();
        services.AddScoped<DashboardSpendingThisWeek>();
        services.AddScoped<DashboardSpendingByCategory>();
        services.AddScoped<ChangeObligorFixedExpense>();
        services.AddScoped<IDateFilterInfo, DateFilterInfo>();
        services.AddScoped<IUserInfo, UserInfo>();
        services.AddScoped<DashboardSpendingByObligor>();
        services.AddScoped<GetUserService>();
        services.AddScoped<GetRequisitionUrlService>();
        services.AddScoped<ConfirmRequisitionService>();
    }

    public static void AddAuth(this IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = "https://auth.stefanaru.com";
                options.Audience = "account";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = "https://auth.stefanaru.com/realms/stefanaru",
                    ValidAudience = "account"
                };

                options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{options.TokenValidationParameters.ValidIssuer}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());
            });

        services.AddAuthorizationBuilder()
            .AddPolicy("PlutusUserPolicy", policy =>
                policy.Requirements.Add(new PlutusUserRequirement()))
            .SetFallbackPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PlutusUserRequirement())
                .Build());
    }

    public static void AddCors(this WebApplication app, IConfiguration configuration)
    {
        app.UseCors(builder =>
        {
            builder.WithOrigins(configuration.GetValue<string>("UiUrl"))
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    }

    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
        scope.ServiceProvider.GetRequiredService<DataMigrator>().MigrateData().Wait();
    }


    public static void UserRequestLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.IncludeQueryInRequestPath = true;
            options.GetLevel = (httpContext, elapsed, ex) => ex != null || httpContext.Response.StatusCode >= 400
                ? LogEventLevel.Error
                : LogEventLevel.Information;
        });
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "plutus").MigrationsAssembly("Plutus.Infrastructure")));
    }

    public static void AddAllDataMigrations(this IServiceCollection services)
    {
        var dataMigrations = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Contains(typeof(IDataMigration)))
            .ToList();

        foreach (var dataMigration in dataMigrations)
        {
            services.AddScoped(typeof(IDataMigration), dataMigration);
        }
    }

    public static void AddSerilog(this WebApplicationBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);
        builder.Host.UseSerilog();
    }

    public static void AddNewtonsoft(this IMvcBuilder builder)
    {
        builder.AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new SortOrderTypeConverter());
            options.SerializerSettings.Converters.Add(new ListFilterTypeConverter());
        });

        JsonConvert.DefaultSettings = () =>
        {
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new ListFilterTypeConverter());
            return settings;
        };
    }

    public static void AddSwagger(this IServiceCollection services)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            services.AddOpenApi();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerServices();
        }
    }
}
