using SisyphusChat.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.DTO;

namespace SisyphusChat.Infrastructure.Repositories
{
    public class ReportRepository(ApplicationDbContext context) : IReportRepository
    {
        public async Task<List<Attachment>> GetAttachments()
            => await context.Attachments.OrderByDescending(a => a.TimeCreated).ToListAsync();

        public async Task<List<Chat>> GetChats()
        {
            var chats = await context.Chats
                .OrderByDescending(c => c.TimeCreated) // Ensure you're ordering by the correct property
                .ToListAsync();

            // Debug logging


            return chats;
        }

        public async Task<List<Message>> GetMessages()
        {
            // Retrieve and order messages by TimeCreated in descending order
            var messages = await context.Messages
                .OrderByDescending(m => m.TimeCreated) // Ensure you're ordering by the correct property
                .ToListAsync();

            // Debug logging to verify order and data
            foreach (var message in messages)
            {
                Console.WriteLine($"Message ID: {message.Id}, TimeCreated: {message.TimeCreated}, Content: {message.Content}, Sender: {message.SenderId}");
            }

            return messages; // Return the ordered list of messages
        }


        public async Task<List<User>> GetUsers()
            => await context.Users.OrderByDescending(u => u.TimeCreated).ToListAsync();

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
                            .OrderByDescending(m => m.TimeCreated)
                            .Select(m => new
                            {
                                Content = m.Content,
                                TimeCreated = m.TimeCreated
                            })
                            .FirstOrDefault()
                    })
                .Select(result => new UserWithLastMessageDto
                {
                    UserId = result.User.Id.ToString(),
                    UserName = result.User.UserName,
                    Email = result.User.Email,
                    LastMessageContent = result.LastMessage != null ? result.LastMessage.Content : "No messages sent",
                    LastMessageDate = result.LastMessage != null ? result.LastMessage.TimeCreated : (DateTime?)null
                })
                .OrderByDescending(u => u.LastMessageDate) // Order by LastMessageDate
                .ToListAsync();

            return usersWithLastMessage;
        }

        public async Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport()
        {
            var attachmentReport = await context.Attachments
                .Select(a => new AttachmentUsageReportDto
                {
                    AttachmentId = a.Id,
                    UserName = a.Message.Sender.UserName,
                    FileName = a.FileName,
                    DateUploaded = a.TimeCreated,
                    RelatedMessageContent = a.Message.Content
                })
                .OrderByDescending(a => a.DateUploaded) // Order by DateUploaded
                .ToListAsync();

            return attachmentReport;
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
                    UserRole = cu.Chat.OwnerID == cu.UserId ? "Owner" : "Member"
                })
                .OrderByDescending(cu => cu.UserName) // You might want to change this to order by a relevant field
                .ToListAsync();

            return chatParticipationReport;
        }

        public async Task<List<MessageReportDto>> GetMessagesReport(ChatType chatType)
        {
            var messageReports = await context.Messages
                .Where(m => m.Chat.Type == chatType)
                .SelectMany(m =>
                    m.Chat.ChatUsers
                    .Where(cu => cu.UserId != m.SenderId)
                    .Select(cu => new MessageReportDto
                    {
                        MessageId = m.Id,
                        SenderUserName = m.Sender.UserName,
                        ReceiverUserName = cu.User.UserName,
                        MessageContent = m.Content,
                        DateSent = m.TimeCreated,
                        ChatType = m.Chat.Type.ToString()
                    }))
                .OrderByDescending(m => m.DateSent) // Order by DateSent
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
                .OrderByDescending(u => u.LastLogin ?? u.LastUpdated)
                .ToListAsync();

            return userActivityReport;
        }
    }
}
