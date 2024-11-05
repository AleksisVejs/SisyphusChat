using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Core.Interfaces
{
    public interface IUserDeletionService
    {
        Task DeleteUserAndRelatedDataAsync(string userId);
        Task DeleteUserNotificationsAsync(string userId);
    }
}
