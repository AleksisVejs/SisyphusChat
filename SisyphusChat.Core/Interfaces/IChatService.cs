using SisyphusChat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Core.Interfaces
{
    public interface IChatService : ICrud<ChatModel>
    {
        Task<ChatModel> OpenOrCreatePrivateChatAsync(string currentUserId, string recipientUserId);

        Task<ICollection<ChatModel>> GetAssociatedChatsAsync(UserModel currentUser);

        Task DeleteUserFromChatByIdAsync(string userId, string chatId);

        Task UpdateWithMembersAsync(ChatModel model, ICollection<string> newMembers);
    }
}
