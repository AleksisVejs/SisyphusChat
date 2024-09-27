using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Attachments")]
public class Attachment : BaseEntity
{
    [Required]
    public Guid MessageID { get; set; }

    [ForeignKey(nameof(MessageID))]
    public Message Message { get; set; }

    [Required]
    public byte[] Content { get; set; }

    [Required]
    public string FileName { get; set; }
}
