using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SisyphusChat.Web.Models;
using SisyphusChat.Infrastructure.Entities;


public class BanExpirationService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BanExpirationService> _logger;

    public BanExpirationService(
        IServiceProvider services,
        ILogger<BanExpirationService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

                var bannedUsers = await userManager.Users
                    .Where(u => u.IsBanned 
                           && u.BanEnd.HasValue  // Only get temporary bans
                           && u.BanStart.HasValue
                           && u.BanEnd <= DateTime.UtcNow)
                    .ToListAsync(stoppingToken);

                foreach (var user in bannedUsers)
                {
                    user.IsBanned = false;
                    user.BanStart = null;
                    user.BanEnd = null;
                    await userManager.UpdateAsync(user);
                    
                    _logger.LogInformation("User {UserId} automatically unbanned due to ban expiration", user.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ban expirations");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
} 