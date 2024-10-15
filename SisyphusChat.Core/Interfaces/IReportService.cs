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

        // All methods are asynchronous to avoid blocking the main thread and improve performance

        public Task<List<Attachment>> GetAttachments(); // For admin to get all attachments
        public Task<List<Chat>> GetChats(); // For admin to get all chats
        public Task<List<Message>> GetMessages(); // For admin to get all messages
        public Task<List<User>> GetUsers(); // For admin to get all users

        public Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage(); // To track users with their last sent message for activity purposes
        public Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport(); // To track attachment usage by user, file name, date uploaded, and related message content
        public Task<List<MessageReportDto>> GetMessageReport(ChatType chatType); // To track messages sent by user, message content, date sent, and status sensitive data, no privacy
        public Task<List<UserActivityReportDto>> GetUserActivities(); // To track user activities, last login, last updated, and online status, messages sent all time
        public Task<List<ChatParticipationReportDto>> GetChatParticipationReport(); // To track user participation in chats by chat title, user name, and user role
        public Task<byte[]> GeneratePdfAsync(string reportType); // To generate a PDF report based on the report type, all of the above in byte array

    }
}
