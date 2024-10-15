using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Infrastructure.DTO
{
    public class MessageReportDto
    {
        public Guid MessageId { get; set; }
        public string SenderUserName { get; set; }
        public string ReceiverUserName { get; set; }
        public string MessageContent { get; set; }
        public DateTime DateSent { get; set; }
        public string Status { get; set; } // E.g., "Sent", "Delivered", "Read"

        public string ChatType { get; set; }
    }

}
