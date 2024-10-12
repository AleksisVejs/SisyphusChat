using SisyphusChat.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using SisyphusChat.Infrastructure.DTO;

namespace SisyphusChat.Core.Services
{
    public class ReportService : IReportService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Attachment>> GetAttachments()
        {
            var attachments = await _unitOfWork.AttachmentRepository.GetAllAsync();
            return attachments.ToList();
        }

        public async Task<List<Chat>> GetChats()
        {
            var chats = await _unitOfWork.ChatRepository.GetAllAsync();
            return chats.ToList();
        }

        public async Task<List<Message>> GetMessages()
        {
            var messages = await _unitOfWork.MessageRepository.GetAllAsync();
            return messages.ToList();
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();
            return users.ToList();
        }

        // Generate PDF instead of Excel
        public async Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage()
        {
            var usersWithLastMessage = await _unitOfWork.ReportRepository.GetUsersWithLastMessage();
            return usersWithLastMessage;
        }
        public async Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport()
        {
            var attachmentsUsageReport = await _unitOfWork.ReportRepository.GetAttachmentsUsageReport();
            return attachmentsUsageReport;
        }
        public async Task<List<MessageReportDto>> GetMessageReport()
        {
            var messageReport = await _unitOfWork.ReportRepository.GetMessagesReport();
            return messageReport;
        }
        public async Task<List<UserActivityReportDto>> GetUserActivities()
        {
            var userActivities = await _unitOfWork.ReportRepository.GetUsersActivityReport();
            return userActivities;
        }
        public async Task<List<ChatParticipationReportDto>> GetChatParticipationReport()
        {
            var chatParticipationReport = await _unitOfWork.ReportRepository.GetChatParticipationReports();
            return chatParticipationReport;
        }
        public async Task<MemoryStream> GeneratePdfAsync(string reportType)
        {
            MemoryStream memoryStream = new MemoryStream();

            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
            writer.CloseStream = false;

            document.Open();
            Paragraph title = new Paragraph($"{reportType} Report", new Font(Font.FontFamily.HELVETICA, 18, Font.BOLD));
            title.Alignment = Element.ALIGN_CENTER; // Align center
            document.Add(title);
            document.Add(new Paragraph(" "));

            
            PdfPTable table;
            switch (reportType)
            {
                case "Attachments":
                    table = new PdfPTable(3); // 3 columns for Attachments
                    var attachments = await GetAttachments();
                    AddTableHeaders(table, "ID", "FileName", "DateUploaded");
                    foreach (var attachment in attachments)
                    {
                        table.AddCell(attachment.Id.ToString());
                        table.AddCell(attachment.FileName);
                        table.AddCell(attachment.TimeCreated.ToString());
                    }
                    break;
                case "Chats":
                    table = new PdfPTable(3); // 3 columns for Chats
                    var chats = await GetChats();
                    AddTableHeaders(table, "ID", "Title", "DateCreated");
                    foreach (var chat in chats)
                    {
                        table.AddCell(chat.Id.ToString());
                        table.AddCell(chat.Name);
                        table.AddCell(chat.TimeCreated.ToString());
                    }
                    break;
                case "Messages":
                    table = new PdfPTable(3); // 3 columns for Messages
                    var messages = await GetMessages();
                    AddTableHeaders(table, "ID", "Content", "DateSent");
                    foreach (var message in messages)
                    {
                        table.AddCell(message.Id.ToString());
                        table.AddCell(message.Content);
                        table.AddCell(message.LastUpdated.ToString());
                    }
                    break;
                case "Users":
                    table = new PdfPTable(3); // 3 columns for Users
                    var users = await GetUsers();
                    AddTableHeaders(table, "ID", "UserName", "Email");
                    foreach (var user in users)
                    {
                        table.AddCell(user.Id.ToString());
                        table.AddCell(user.UserName);
                        table.AddCell(user.Email);
                    }
                    break;
                case "UsersWithLastMessage":
                    table = new PdfPTable(6); // Set to 6 columns for UsersWithLastMessage
                    AddTableHeaders(table, "ID", "UserName", "Email", "LastMessageContent", "LastMessageSenderId", "LastMessageDate");
                    var usersWithLastMessage = await GetUsersWithLastMessage();
                    foreach (var user in usersWithLastMessage)
                    {
                        table.AddCell(user.UserId.ToString());
                        table.AddCell(user.UserName);
                        table.AddCell(user.Email);
                        table.AddCell(user.LastMessageContent);
                        table.AddCell(user.LastMessageSenderId);
                        table.AddCell(user.LastMessageDate?.ToString("g")); // Format the date appropriately
                    }
                    break;

                case "AttachmentsUsageReport":
                    table = new PdfPTable(5); // Set to 5 columns for AttachmentsUsageReport
                    AddTableHeaders(table, "AttachmentId", "UserName", "FileName", "DateUploaded", "RelatedMessageContent");
                    var attachmentReport = await _unitOfWork.ReportRepository.GetAttachmentsUsageReport();
                    foreach (var attachment in attachmentReport)
                    {
                        table.AddCell(attachment.AttachmentId.ToString());
                        table.AddCell(attachment.UserName);
                        table.AddCell(attachment.FileName);
                        table.AddCell(attachment.DateUploaded.ToString());
                        table.AddCell(attachment.RelatedMessageContent);
                    }
                    break;
                case "ChatParticipationReports":
                    table = new PdfPTable(4); // Set to 3 columns for ChatParticipationReports
                    AddTableHeaders(table, "ChatId", "ChatTitle", "UserName", "UserRole");
                    var chatParticipationReport = await _unitOfWork.ReportRepository.GetChatParticipationReports();
                    foreach (var chat in chatParticipationReport)
                    {
                        table.AddCell(chat.ChatId.ToString());
                        table.AddCell(chat.ChatTitle);
                        table.AddCell(chat.UserName);
                        table.AddCell(chat.UserRole);
                    }
                    break;
                case "MessageReportDto":
                    table = new PdfPTable(6); // Set to 4 columns for MessageReportDto
                    AddTableHeaders(table, "MessageId", "SenderUserName", "ReceiverUsername", "MessageContent","DateSent","Status");
                    var messageReport = await _unitOfWork.ReportRepository.GetMessagesReport();
                    foreach (var message in messageReport)
                    {
                        table.AddCell(message.MessageId.ToString());
                        table.AddCell(message.SenderUserName);
                        table.AddCell(message.ReceiverUserName);
                        table.AddCell(message.MessageContent);
                        table.AddCell(message.DateSent.ToString());
                        table.AddCell(message.Status);
                    }
                    break;
                case "UserActivityReport":
                    table = new PdfPTable(5); // Set to 5 columns for UserActivityReport
                    AddTableHeaders(table, "UserName", "LastLogin", "LastUpdated", "IsOnline", "TotalMessagesSent");
                    var userActivityReport = await _unitOfWork.ReportRepository.GetUsersActivityReport();
                    foreach (var user in userActivityReport)
                    {
                        table.AddCell(user.UserName);
                        table.AddCell(user.LastLogin.ToString());
                        table.AddCell(user.LastUpdated.ToString());
                        table.AddCell(user.IsOnline ? "Online" : "Offline"); // Convert boolean to string
                        table.AddCell(user.TotalMessagesSent.ToString());
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid report type");
            }

            // Add the table to the document
            document.Add(table);
            document.Close();

            memoryStream.Position = 0; // Reset the stream position for reading
            return memoryStream;
        }



        // Helper method to add table headers
        private void AddTableHeaders(PdfPTable table, params string[] headers)
        {
            foreach (var header in headers)
            {
                PdfPCell cell = new PdfPCell(new Phrase(header))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY
                };
                table.AddCell(cell);
            }
        }
    }
}
