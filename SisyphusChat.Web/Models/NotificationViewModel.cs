using SisyphusChat.Core.Models;
using System.Collections.Generic;

namespace SisyphusChat.Web.Models
{
    public class NotificationViewModel
    {
        public List<NotificationModel> Notifications { get; set; } = new List<NotificationModel>();

        // Property to get the count of notifications
        public int Count => Notifications?.Count ?? 0;

        public NotificationModel Notification { get; set; } = new NotificationModel();
    }
}
