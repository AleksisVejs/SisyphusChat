using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SisyphusChat.Core.Models
{
    public class NotificationModel
    {
        public string Id { get; set; }
        public string UserId { get; set; } // Reference to the user

        [JsonIgnore] // This will prevent the User object from being serialized
        public UserModel User { get; set; }
        public string NotificationType { get; set; }
        public string SenderUsername { get; set; }

        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; } // To track if the notification has been read

        public string ChatId { get; set; }
    }

}
