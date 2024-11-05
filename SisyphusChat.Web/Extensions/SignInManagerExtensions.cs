using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Data;

public static class SignInManagerExtensions
{
    public static async Task<SignInResult> CheckBanStatusAsync<TUser>(
        this SignInManager<TUser> signInManager,
        string email,
        ApplicationDbContext context) where TUser : class
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));
            
        if (string.IsNullOrEmpty(email))
            return SignInResult.Failed;

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());

        if (user != null && user.IsBanned)
        {
            if (user.BanStart.HasValue)
            {
                if (user.BanEnd.HasValue)
                {
                    if (user.BanEnd > DateTime.UtcNow)
                        return SignInResult.LockedOut; // Active temporary ban
                    else
                    {
                        // Ban expired, auto-unban
                        user.IsBanned = false;
                        user.BanStart = null;
                        user.BanEnd = null;
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    return SignInResult.NotAllowed; // Permanent ban
                }
            }
        }

        return SignInResult.Success;
    }
} 