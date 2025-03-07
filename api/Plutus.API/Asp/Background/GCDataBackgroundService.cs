using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Services;

namespace Plutus.API.Services;

public class GCDataBackgroundService(ILogger<GCDataBackgroundService> logger, IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private readonly ILogger<GCDataBackgroundService> _logger = logger;
    private Timer _timer;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(15));
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            _logger.LogInformation("Development environment, not running GC Data Background Service.");
            return;
        }

        _logger.LogInformation("GC Data Background Service is being executed.");

        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var gCDataCollection = scope.ServiceProvider.GetRequiredService<GCDataCollection>();

        var lastRequest = dbContext.GoCardlessRequests.OrderByDescending(x => x.MadeAt).FirstOrDefault();

        if (lastRequest == null || lastRequest.MadeAt < DateTime.UtcNow.AddHours(-6))
        {
            _logger.LogInformation("Making a new GoCardless request.");
            await gCDataCollection.CollectData();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }
}

