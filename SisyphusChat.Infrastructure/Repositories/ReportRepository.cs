using SisyphusChat.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.DTO;

namespace SisyphusChat.Infrastructure.Repositories
{
    public class ReportRepository(ApplicationDbContext context) : IReportRepository
    {
        public async Task<List<Attachment>> GetAttachments() => await context.Attachments.ToListAsync();
        public async Task<List<Chat>> GetChats() => await context.Chats.ToListAsync();

        public async Task<List<Message>> GetMessages() => await context.Messages.ToListAsync();
        public async Task<List<User>> GetUsers() => await context.Users.ToListAsync();
        public async Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage()
        {
            var usersWithLastMessage = await context.Users
                .GroupJoin(
                    context.Messages,
                    user => user.Id,
                    message => message.SenderId,
                    (user, messages) => new
                    {
                        User = user,
                        LastMessage = messages
                            .OrderByDescending(m => m.LastUpdated)
                            .Select(m => new
                            {
                                Content = m.Content,
                                LastUpdated = m.LastUpdated
                            })
                            .FirstOrDefault()
                    })
                .Select(result => new UserWithLastMessageDto
                {
                    UserId = result.User.Id.ToString(),
                    UserName = result.User.UserName,
                    Email = result.User.Email,
                    LastMessageContent = result.LastMessage != null ? result.LastMessage.Content : "No messages sent", // Handle null content
                    LastMessageDate = result.LastMessage != null ? result.LastMessage.LastUpdated : (DateTime?)null // Leave null if no messages
                })
                .ToListAsync();

            return usersWithLastMessage;
        }


        public async Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport()
        {
            var attachmentReport = await context.Attachments
                .Select(a => new AttachmentUsageReportDto
                {
                    AttachmentId = a.Id,
                    UserName = a.Message.Sender.UserName, // Get the sender's username from the related Message
                    FileName = a.FileName,
                    DateUploaded = a.TimeCreated, // Assuming TimeCreated tracks the upload date
                    RelatedMessageContent = a.Message.Content // Get the content of the associated Message
                })
                .ToListAsync();

            return attachmentReport;

        }
        public async Task<List<ChatParticipationReportDto>> GetChatParticipationReports()
        {
            var chatParticipationReport = await context.ChatUsers
                .Select(cu => new ChatParticipationReportDto
                {
                    ChatId = cu.ChatId,
                    ChatTitle = cu.Chat.Name,
                    UserName = cu.User.UserName,
                    UserRole = cu.Chat.OwnerID == cu.UserId ? "Owner" : "Member",
                })
                .ToListAsync();

            return chatParticipationReport;
        }
        public async Task<List<MessageReportDto>> GetMessagesReport()
        {
            var messageReports = await context.Messages
                .SelectMany(m =>
                    m.Chat.ChatUsers.Select(cu => new MessageReportDto
                    {
                        MessageId = m.Id,
                        SenderUserName = m.Sender.UserName,
                        ReceiverUserName = cu.User.UserName, // Get the UserName for each ChatUser
                        MessageContent = m.Content,
                        DateSent = m.LastUpdated != DateTime.MinValue ? m.LastUpdated : m.TimeCreated,
                        Status = m.Status.ToString()
                    }))
                .ToListAsync();

            return messageReports;
        }
        public async Task<List<UserActivityReportDto>> GetUsersActivityReport()
        {
            var userActivityReport = await context.Users
             .Select(u => new UserActivityReportDto
             {
                 UserName = u.UserName,
                 LastLogin = u.LastLogin,
                 LastUpdated = u.LastUpdated,
                 IsOnline = u.IsOnline,
                 TotalMessagesSent = context.Messages
                     .Where(m => m.SenderId == u.Id)
                     .Count()
             })
             .ToListAsync();

            return userActivityReport;







        }
    }
}


