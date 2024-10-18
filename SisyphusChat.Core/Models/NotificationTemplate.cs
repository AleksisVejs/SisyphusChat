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
        { "FriendRequest", (username) => $"{username} sent you a friend request." }, // make it persistent
        { "UnseenMessage", (username) => $"You have a new message from {username}." }, // make it timed for a week i guess but it does not store in bell icon when you open it or see in chat its over
        { "Mentioned", (username) => $"You were mentioned by {username} in a message." }, // make it persistent
            {"SystemUpdate",(_)=> $"You have a new system update from Sisyphus." } // make it persistent
    };
    }

}
