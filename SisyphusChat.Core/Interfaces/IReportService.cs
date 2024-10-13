using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.DTO;

namespace SisyphusChat.Core.Interfaces
{
    public interface IReportService
    {
        public Task<List<Attachment>> GetAttachments();
        public Task<List<Chat>> GetChats();
        public Task<List<Message>> GetMessages();
        public Task<List<User>> GetUsers();

        public Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage();
        public Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport();
        public Task<List<MessageReportDto>> GetMessageReport();
        public Task<List<UserActivityReportDto>> GetUserActivities();
        public Task<List<ChatParticipationReportDto>> GetChatParticipationReport();
        public Task<byte[]> GeneratePdfAsync(string reportType);

    }
}
