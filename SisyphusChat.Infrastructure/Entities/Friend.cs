using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Friends")]
public class Friend
{
    [Required]
    public string ReqSenderId { get; set; }

    [Required]
    [ForeignKey(nameof(ReqSenderId))]
    public User ReqSender { get; set; }

    [Required]
    public string ReqReceiverId { get; set; }

    [Required]
    [ForeignKey(nameof(ReqReceiverId))]
    public User ReqReceiver { get; set; }

    [Required]
    public bool IsAccepted { get; set; }
}
