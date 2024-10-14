using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.DTO;

namespace SisyphusChat.Infrastructure.Interfaces
{
    public interface IReportRepository
    {
        
        Task<List<Attachment>> GetAttachments();
        Task<List<Chat>> GetChats();
        Task<List<Message>> GetMessages();
        Task<List<User>> GetUsers();
        // The actual useful methods
        Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage();
        Task<List<UserActivityReportDto>> GetUsersActivityReport();
        Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport();
        Task<List<ChatParticipationReportDto>> GetChatParticipationReports();
        Task<List<MessageReportDto>> GetMessagesReport();



    }
}
