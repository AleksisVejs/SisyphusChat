using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Messages")]
public class Message : BaseEntity
{
    [Required]
    public Guid ChatId { get; set; }
    
    [Required]
    [ForeignKey(nameof(ChatId))]
    public Chat Chat { get; set; }
    
    [Required]
    public Guid SenderId { get; set; }
    
    [Required]
    [ForeignKey(nameof(SenderId))]
    public User Sender { get; set; }
    
    [Required]
    public Blob Content { get; set; }

    [Required]
    public Enum Status { get; set; }
    
    [Required]
    public bool IsReported { get; set; }
}