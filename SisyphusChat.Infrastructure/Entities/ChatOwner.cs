using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("ChatOwners")]
public class ChatOwner
{
    [Required]
    public string ChatId { get; set; }
    
    [Required]
    [ForeignKey(nameof(ChatId))]
    public Chat Chat { get; set; }
    
    [Required]
    public string OwnerId { get; set; }
    
    [Required]
    [ForeignKey(nameof(OwnerId))]
    public User Owner { get; set; }
}