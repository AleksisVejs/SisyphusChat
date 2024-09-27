using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Models
{
    public class AttachmentModel
    {
        public string ID { get; set; }

        public MessageModel MessageID { get; set; }

        public string FileName { get; set; }

        public byte[] Content { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}