using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Chats")]
public class Chat : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }
    
    [Required]
    public ChatType Type { get; set; }
    
    // Navigation properties
    
    public ChatOwner Owner { get; set; } 

    public ICollection<ChatUser> Members { get; set; } 
    
    public ICollection<Message> Messages { get; set; } 
}