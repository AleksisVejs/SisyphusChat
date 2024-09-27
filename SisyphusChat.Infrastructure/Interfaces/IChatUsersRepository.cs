using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Infrastructure.Interfaces
{
    internal interface IChatUsersRepository
    {
        Task AddUserToChatAsync(string chatId, string userId);

        Task RemoveUserFromChatAsync(string chatId, string userId);

        Task<IEnumerable<string>> GetChatUsersAsync(string chatId);
    }
}
