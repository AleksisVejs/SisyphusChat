using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Core.Models
{
    public class NotificationModel
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Reference to the user
        public UserModel User { get; set; }
        public string NotificationType { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; } // To track if the notification has been read
    }

}
