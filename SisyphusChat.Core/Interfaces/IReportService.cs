using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Interfaces
{
    public interface IReportService
    {
        public Task<List<Attachment>> GetAttachments();
        public Task<List<Chat>> GetChats();
        public Task<List<Message>> GetMessages();
        public Task<List<User>> GetUsers();
        public Task<MemoryStream> GenerateExcelAsync(string reportType);

    }
}
