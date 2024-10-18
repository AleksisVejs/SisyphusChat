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
        /*
        Asynchronously retrieves a list of all attachments from the database.
        This method is essential for providing access to the attachments users have uploaded,
        allowing for reporting and management of file usage within the application.
        */
        public async Task<List<Attachment>> GetAttachments() => await context.Attachments.ToListAsync();

        // Asynchronously retrieves a list of all chats from the database.
        // This enables features that require chat data, such as reporting on chat activity
        // or displaying available chat rooms to users.
        public async Task<List<Chat>> GetChats() => await context.Chats.ToListAsync();

        // Asynchronously retrieves a list of all messages from the database.
        // Collecting messages is critical for analyzing user interactions and chat history,
        // which can be leveraged in reporting functionalities.
        public async Task<List<Message>> GetMessages() => await context.Messages.ToListAsync();

        // Asynchronously retrieves a list of all users from the database.
        // This method is fundamental for user management features and analytics, enabling
        // insights into user engagement and activity within the application.
        public async Task<List<User>> GetUsers() => await context.Users.ToListAsync();

        // Asynchronously retrieves a list of users along with their last sent message.
        // This report is particularly useful for displaying user status and their most recent
        // interactions, providing a quick overview of user engagement and communication trends.
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
                            .FirstOrDefault() // Retrieves the latest message sent by the user
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

            return usersWithLastMessage; // Return the enriched user list for further processing or display
        }

        // Asynchronously generates a report of attachment usage, including details about the sender and related messages.
        // This method aggregates data regarding attachments, providing insights into which files are most frequently
        // used and by whom, which is valuable for monitoring content and ensuring appropriate usage policies.
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

            return attachmentReport; // Return the usage report for analysis or display
        }

        // Asynchronously generates a report of chat participation, including user roles in each chat.
        // This report is crucial for understanding user dynamics within chats, identifying
        // who the active participants are and their roles, which aids in moderation and engagement strategies.
        public async Task<List<ChatParticipationReportDto>> GetChatParticipationReports()
        {
            var chatParticipationReport = await context.ChatUsers
                .Select(cu => new ChatParticipationReportDto
                {
                    ChatId = cu.ChatId,
                    ChatTitle = cu.Chat.Name,
                    UserName = cu.User.UserName,
                    UserRole = cu.Chat.OwnerId == cu.UserId ? "Owner" : "Member", // Distinguishing between roles
                })
                .ToListAsync();

            return chatParticipationReport; // Provide the report for chat participation analysis
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
                        DateSent = m.TimeCreated,
                        Status = m.Status.ToString(), // Converts the message status to a string representation
                        ChatType = m.Chat.Type.ToString() // Converts enum ChatType to string (e.g., "Private", "Group")
                    }))
                .ToListAsync();

            return messageReports; // Return the complete message report for insights on communication
        }


        // Asynchronously generates a report of user activity, including last login and total messages sent by each user.
        // Understanding user activity is crucial for engagement tracking and identifying active users versus inactive ones,
        // which helps in tailoring communication strategies and improving user retention.
        public async Task<List<UserActivityReportDto>> GetUsersActivityReport()
        {
            var userActivityReport = await context.Users  // Collects all user objects from the database
             .Select(u => new UserActivityReportDto
             {
                 UserName = u.UserName,
                 LastLogin = u.LastLogin,
                 LastUpdated = u.LastUpdated,
                 IsOnline = u.IsOnline,
                 TotalMessagesSent = context.Messages
                     .Where(m => m.SenderId == u.Id)
                     .Count() // Counts the total messages sent by each user
             })
             .ToListAsync();

            return userActivityReport; // Return the user activity report for analysis
        }
    }
}
