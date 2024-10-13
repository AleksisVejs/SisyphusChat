using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Infrastructure.Interfaces
{
    public interface IReportRepository
    {
        //Task<List<(Attachment,Chat,ChatUser,Friend,Message,User)>> GetAllDataAsync();
        Task<List<Attachment>> GetAttachments();
        Task<List<Chat>> GetChats();
        Task<List<Message>> GetMessages();
        Task<List<User>> GetUsers();


    }
}
