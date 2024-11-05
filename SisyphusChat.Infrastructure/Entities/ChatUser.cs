using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("ChatUsers")]
public class ChatUser
{
    [Required]
    public string ChatId { get; set; }
    
    [Required]
    [ForeignKey(nameof(ChatId))]
    public Chat Chat { get; set; }
    
    [Required]
    public string UserId { get; set; }
    
    [Required]
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}