using Microsoft.EntityFrameworkCore;
using Plutus.Infrastructure.Data;
using Plutus.Infrastructure.Data.Entities;
using Plutus.Infrastructure.Enums;
using Serilog;

namespace Plutus.API.Asp.Background;

public class RequistionExpirationBackgroundService(IServiceScopeFactory serviceScopeFactory) : IHostedService, IDisposable
{
    private Timer _timer;

    public void Dispose()
    {
        _timer?.Dispose();
        GC.SuppressFinalize(this);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(6));
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Change(Timeout.Infinite, 0);
        return Task.CompletedTask;
    }

    private async void DoWork(object state)
    {
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            Log.Information("Development environment, not running GC Data Background Service.");
            return;
        }

        Log.Information($"{nameof(RequistionExpirationBackgroundService)} is being executed.");

        using var scope = serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var usersWithExpiringRequisitions = await dbContext.Requisitions
            .Where(x => x.IsConfirmed && x.Created.Date < DateTime.UtcNow.AddDays(-179))
            .Select(x => x.UserId)
            .ToListAsync();

        await dbContext.Users
            .Where(x => usersWithExpiringRequisitions.Contains(x.Id))
            .UpdateFromQueryAsync(x => new User
            {
                Id = x.Id,
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName,
                UserName = x.UserName,
                Name = x.Name,
                Status = UserStatus.RequisitionExpired,
                RevolutAccountId = x.RevolutAccountId
            });
    }
}
