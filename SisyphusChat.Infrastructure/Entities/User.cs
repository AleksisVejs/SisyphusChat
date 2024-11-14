using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SisyphusChat.Infrastructure.Entities
{
    [Index(nameof(UserName), nameof(Email), nameof(PasswordHash), IsUnique = true)]
    public class User : IdentityUser
    {
        public byte[]? Picture { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsBanned { get; set; }
        public bool IsDeleted { get; set; }

        public DateTime? BanStart { get; set; }

        public DateTime? BanEnd { get; set; }

        [Required]
        public bool IsOnline { get; set; }      

        [Required]
        public DateTime TimeCreated { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }

        public DateTime? LastLogin { get; set; }
        public bool IsProfanityEnabled { get; set; } = true;

        // Navigation properties for EF Core

        public ICollection<Chat> Chats { get; set; } = new List<Chat>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();

        public ICollection<Friend> Friends { get; set; } = new List<Friend>();
    }
}
