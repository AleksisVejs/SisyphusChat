using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.DTO;

namespace SisyphusChat.Infrastructure.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<Attachment>> GetAttachments();
        Task<List<Chat>> GetChats();
        Task<List<Message>> GetMessages();
        Task<List<User>> GetUsers();
        // The actual useful methods
        Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage();
        Task<List<UserActivityDto>> GetUsersActivityReport();
        Task<List<AttachmentUsageDto>> GetAttachmentsUsageReport();
        Task<List<ChatParticipationDto>> GetChatParticipationReports();
        Task<List<MessageDto>> GetMessagesReport(ChatType chatType);



    }
}
