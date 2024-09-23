using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SisyphusChat.Infrastructure.Entities
{
    [Index(nameof(UserName), nameof(Email), nameof(PasswordHash), IsUnique = true)]
    [Table("User")]
    public class User : IdentityUser
    {
        public byte[] Picture { get; set; }

        [Required]
        public bool IsOnline { get; set; }

        public ICollection<ChatUser> ChatUsers { get; set; } = new List<ChatUser>();

        public ICollection<Message> Messages { get; set; } = new List<Message>();

        [Required]
        public DateTime TimeCreated { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; }
    }
}
