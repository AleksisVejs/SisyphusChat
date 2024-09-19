using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Friends")]
public class Friend : BaseEntity
{
    [Required]
    public string ReqSenderID { get; set; }

    [ForeignKey(nameof(ReqSenderID))]
    public User ReqSender { get; set; }

    [Required]
    public string ReqReceiverID { get; set; }

    [ForeignKey(nameof(ReqReceiverID))]
    public User ReqReceiver { get; set; }

    [Required]
    public bool IsAccepted { get; set; }
}
