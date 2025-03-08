using Serilog;
using Plutus.API;
using Plutus.API.Asp.Background;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddServices();
builder.Services.AddAuth();
builder.Services.AddSwagger();
builder.Services.AddControllers()
    .AddNewtonsoft();

builder.Services.AddHostedService<GCDataBackgroundService>();

builder.Services.AddDatabase(builder.Configuration);

var app = builder.Build();

app.MigrateDb();

app.UserRequestLogging();

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

app.AddCors(builder.Configuration);
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

Console.WriteLine("API starting");

app.Run();
