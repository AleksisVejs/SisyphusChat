using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace SisyphusChat.Infrastructure.Entities;

[Index(nameof(UserName), nameof(Email), nameof(PasswordHash), IsUnique = true)]
public class User : IdentityUser
{
    public DateTime LastLoginAt { get; set; }

    public bool IsOnline { get; set; }

    // Navigation properties

    public ICollection<Chat> Chats { get; set; }
}
