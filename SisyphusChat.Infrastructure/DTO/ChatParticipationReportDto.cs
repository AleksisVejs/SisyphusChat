using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Infrastructure.DTO
{
    public class ChatParticipationReportDto
    {
        // Custom data transfer object for chat participation reports for specific data
        public Guid ChatId { get; set; }
        public string ChatTitle { get; set; }
        public string UserName { get; set; }
        public string UserRole { get; set; } // E.g., "Admin", "Member"
    }

}
