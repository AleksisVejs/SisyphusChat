using SisyphusChat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Core.Interfaces
{
    public interface IUserService : ICrud<UserModel>
    {
        Task<UserModel> GetCurrentContextUserAsync();

        Task<ICollection<UserModel>> GetAllUsersAsync();

        Task<ICollection<ChatUserModel>> GetUsersByUserNamesAsync(string[] userNamesArray);

        Task<ICollection<UserModel>> GetAllExceptCurrentUserAsync(UserModel currentUser);

        Task SetUserOnlineAsync(string userId);

        Task SetUserOfflineAsync(string userId);
    }
}
