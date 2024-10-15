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

        // Asynchronously generates a report of messages, detailing each message's sender, receiver, and status.
        // This comprehensive report allows for thorough analysis of messaging patterns, which is essential for
        // understanding communication trends and addressing any issues that arise in messaging.
        public async Task<List<MessageReportDto>> GetMessagesReport(ChatType chatType)
        {
            var messageReports = await context.Messages
                .Where(m => m.Chat.Type == chatType) // Filter by chat type
                .SelectMany(m =>
                    m.Chat.ChatUsers
                    .Where(cu => cu.UserId != m.SenderId) // Exclude the sender from the receiver list
                    .Select(cu => new MessageReportDto
                    {
                        MessageId = m.Id,
                        SenderUserName = m.Sender.UserName,
                        ReceiverUserName = cu.User.UserName, // Get the UserName for each ChatUser (excluding sender)
                        MessageContent = m.Content,
                        DateSent = m.LastUpdated != DateTime.MinValue ? m.LastUpdated : m.TimeCreated,
                        Status = m.Status.ToString(), // Converts the message status to a string representation
                        ChatType = m.Chat.Type.ToString() // Converts enum ChatType to string (e.g., "Private", "Group")
                    }))
                .ToListAsync();

            return messageReports;
        }


        // Asynchronously generates a report of user activity, including last login and total messages sent by each user.
        // Understanding user activity is crucial for engagement tracking and identifying active users versus inactive ones,
        // which helps in tailoring communication strategies and improving user retention.
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


