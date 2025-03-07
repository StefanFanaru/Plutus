using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Plutus.Infrastructure.Abstractions;
using Plutus.Infrastructure.Business.Categories;
using Plutus.Infrastructure.Business.Dashboard;
using Plutus.Infrastructure.Business.Obligors;
using Plutus.Infrastructure.Business.Transactions;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Services;
using Plutus.Infrastructure.Common;
using Plutus.API.Services;
using Plutus.API.Asp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new SortOrderTypeConverter());
    options.SerializerSettings.Converters.Add(new ListFilterTypeConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton<IAuthorizationHandler, PlutusUserHandler>();
builder.Services.AddScoped<DataMigrator>();
builder.Services.AddScoped<GCAuth>();
builder.Services.AddScoped<GCGetData>();
builder.Services.AddScoped<GCDataCollection>();
builder.Services.AddScoped<GCInsertData>();
builder.Services.AddScoped<InsertSampleData>();
builder.Services.AddScoped<ListObligors>();
builder.Services.AddScoped<DashboardStats>();
builder.Services.AddScoped<DashboardSpendingStats>();
builder.Services.AddScoped<ListTransactions>();
builder.Services.AddScoped<ListCategories>();
builder.Services.AddScoped<ExcludeTransaction>();
builder.Services.AddScoped<ChangeTransactionCategory>();
builder.Services.AddScoped<SplitTransaction>();
builder.Services.AddScoped<UnsplitTransaction>();
builder.Services.AddScoped<DashboardSpendingThisWeek>();
builder.Services.AddScoped<DashboardSpendingByCategory>();
builder.Services.AddScoped<ChangeObligorFixedExpense>();
builder.Services.AddScoped<IDateFilterInfo, DateFilterInfo>();
builder.Services.AddScoped<DashboardSpendingByObligor>();

AddAllDataMigrations();

void AddAllDataMigrations()
{
    var dataMigrations = Assembly.GetExecutingAssembly().GetTypes()
        .Where(t => t.GetInterfaces().Contains(typeof(IDataMigration)))
        .ToList();

    foreach (var dataMigration in dataMigrations)
    {
        builder.Services.AddScoped(typeof(IDataMigration), dataMigration);
    }
}

builder.Services.AddHostedService<GCDataBackgroundService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "plutus").MigrationsAssembly("Plutus.Infrastructure")));


// Add services to the container.
builder.Services.AddAuthentication(options =>
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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("PlutusUserPolicy", policy =>
            policy.Requirements.Add(new PlutusUserRequirement()))
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new PlutusUserRequirement())
        .Build());

var app = builder.Build();

JsonConvert.DefaultSettings = () =>
{
    var settings = new JsonSerializerSettings();
    settings.Converters.Add(new ListFilterTypeConverter());
    return settings;
};

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
    scope.ServiceProvider.GetRequiredService<DataMigrator>().MigrateData().Wait();

    // scope.ServiceProvider.GetRequiredService<InsertSampleData>().InsertSampleDataAsync().Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config =>
    {
        config.OAuthClientId("plutus-local");
    });
    app.Map("/swagger", swaggerApp =>
    {
        swaggerApp.UseSwagger();
        swaggerApp.UseSwaggerUI();
    });
}

// allow cors from https://plutus.hl.stefanaru.com
app.UseCors(builder =>
{
    builder.WithOrigins("https://192.168.34.245:3101")
        .AllowAnyHeader()
        .AllowAnyMethod();
    builder.WithOrigins("https://plutus.hl.stefanaru.com")
        .AllowAnyHeader()
        .AllowAnyMethod();
    builder.WithOrigins("https://plutus-docker.hl.stefanaru.com")
        .AllowAnyHeader()
        .AllowAnyMethod();
    builder.WithOrigins("https://plutus.stefanaru.com")
        .AllowAnyHeader()
        .AllowAnyMethod();
});
app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
// use controllers
app.MapControllers();


// setup authentication and authorization for Bearer token from https://auth.stefanaru.com
app.UseAuthentication();
app.UseAuthorization();




Console.WriteLine("API starting");

app.Run();

