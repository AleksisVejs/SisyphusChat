using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Infrastructure.DTO
{
    public class UserActivityDto
    {
        // Custom data transfer object for user activity reports for specific data
        public string UserName { get; set; }
        public int TotalMessagesSent { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsOnline { get; set; }
    }

}
