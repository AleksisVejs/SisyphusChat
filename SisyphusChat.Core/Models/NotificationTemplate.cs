using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Core.Models
{
    public static class NotificationTemplate
    {
        public static Dictionary<string, Func<string, string>> Templates = new()
    {
        { "FriendRequest", (username) => $"{username} sent you a friend request." },
        { "UnseenMessage", (username) => $"You have a new message from {username}." },
        { "Mentioned", (username) => $"You were mentioned by {username} in a message." },
            {"SystemUpdate",(_)=> $"You have a new system update from Sisyphus." }
    };
    }

}
