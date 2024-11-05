using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class ReportModel
    {
        public string Id { get; set; }

        public string ChatId { get; set; }

        public ChatModel Chat { get; set; }
        
        public string MessageId { get; set; }

        public MessageModel Message { get; set; }

        public string ReportedUserId { get; set; }

        public UserModel ReportedUser { get; set; }

        public ReportType Type { get; set; }
        
        public string Reason { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}