using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Messages")]
public class Message : BaseEntity
{
    [Required]
    public string ChatId { get; set; }
    
    [Required]
    [ForeignKey(nameof(ChatId))]
    public Chat Chat { get; set; }
    
    [Required]
    public string SenderId { get; set; }
    
    [Required]
    [ForeignKey(nameof(SenderId))]
    public User Sender { get; set; }
    
    [Required]
    public string Content { get; set; }

    [Required]
    public MessageStatus Status { get; set; }

    [Required]
    public bool IsEdited { get; set; }
}