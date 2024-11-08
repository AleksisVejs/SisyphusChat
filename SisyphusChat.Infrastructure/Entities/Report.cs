using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisyphusChat.Infrastructure.Entities;

[Table("Reports")]
public class Report : BaseEntity
{
    [Required]
    public string ChatId { get; set; }
    
    [Required]
    [ForeignKey(nameof(ChatId))]
    public Chat Chat { get; set; }

    public string? MessageId { get; set; }

    [ForeignKey(nameof(MessageId))]
    public Message? Message { get; set; }

    [Required]
    public string ReportedUserId { get; set; }
    
    [Required]
    [ForeignKey(nameof(ReportedUserId))]
    public User ReportedUser { get; set; }

    [Required]
    public ReportType Type { get; set; }
    
    public string? Reason { get; set; }
}