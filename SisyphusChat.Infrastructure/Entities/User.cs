using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace SisyphusChat.Infrastructure.Entities;

[Index(nameof(UserName), nameof(Email), nameof(PasswordHash), IsUnique = true)]
public class User : IdentityUser
{
    public byte[] Picture { get; set; }

    [Required]
    public bool IsOnline { get; set; }
}
