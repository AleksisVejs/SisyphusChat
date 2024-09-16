using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Chats")]
public class Chat : BaseEntity
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    
    [Required]
    public Enum Type { get; set; }

    [Required]
    public Guid OwnerID { get; set; }
    
    [Required]
    [ForeignKey(nameof(OwnerID))]
    public User Owner { get; set; } 

    [Required]
    public bool IsReported { get; set; }
}