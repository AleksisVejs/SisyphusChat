using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.DTO;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Services
{
    public class ReportService : IReportService
    {
        // To inject the UnitOfWork dependency with readonly to ensure it is not changed
        private readonly IUnitOfWork _unitOfWork;

        // To inject the UnitOfWork dependency in the constructor
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

        public async Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage()
        {
            return await _unitOfWork.ReportRepository.GetUsersWithLastMessage();
        }

        public async Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport()
        {
            return await _unitOfWork.ReportRepository.GetAttachmentsUsageReport();
        }

        public async Task<List<MessageReportDto>> GetMessageReport(ChatType chatType)
        {
            return await _unitOfWork.ReportRepository.GetMessagesReport(chatType);
        }

        public async Task<List<UserActivityReportDto>> GetUserActivities()
        {
            return await _unitOfWork.ReportRepository.GetUsersActivityReport();
        }

        public async Task<List<ChatParticipationReportDto>> GetChatParticipationReport()
        {
            return await _unitOfWork.ReportRepository.GetChatParticipationReports();
        }

        // To generate a PDF report table based on the report type asynchronously and the methods used are above
        public async Task<byte[]> GeneratePdfAsync(string reportType)
        {
            // To create a memory stream to store the PDF using "using" to dispose of it properly after use and avoid memory leaks, errors
            using (var memoryStream = new MemoryStream())
            {
                // To initialize PDF writer and document
                using (var writer = new PdfWriter(memoryStream))
                using (var pdf = new PdfDocument(writer))
                using (var document = new Document(pdf))
                {
                    // To add the title
                    Paragraph title = new Paragraph($"{reportType} Report")
                        .SetFontSize(18)
                        .SetBold()
                        .SetTextAlignment(TextAlignment.CENTER);
                    document.Add(title);
                    document.Add(new Paragraph(" ")); // Add some space

                    Table table;

                    switch (reportType)
                    {
                        case "Attachments":
                            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth(); // 3 columns for Attachments
                            var attachments = await GetAttachments();
                            AddTableHeaders(table, "ID", "FileName", "DateUploaded");
                            foreach (var attachment in attachments)
                            {
                                table.AddCell(attachment.Id.ToString());
                                table.AddCell(attachment.FileName);
                                table.AddCell(attachment.TimeCreated.ToString("g")); // Use a date format if needed
                            }
                            break;

                        case "Chats":
                            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth(); // 3 columns for Chats
                            var chats = await GetChats();
                            AddTableHeaders(table, "ID", "Title", "DateCreated");
                            foreach (var chat in chats)
                            {
                                table.AddCell(chat.Id.ToString());
                                table.AddCell(chat.Name);
                                table.AddCell(chat.TimeCreated.ToString("g"));
                            }
                            break;

                        case "Messages":
                            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth(); // 3 columns for Messages
                            var messages = await GetMessages();
                            AddTableHeaders(table, "ID", "Content", "DateSent");
                            foreach (var message in messages)
                            {
                                table.AddCell(message.Id.ToString());
                                table.AddCell(message.Content);
                                table.AddCell(message.LastUpdated.ToString("g"));
                            }
                            break;

                        case "Users":
                            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth(); // 3 columns for Users
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
                            table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth(); // 5 columns for UsersWithLastMessage
                            AddTableHeaders(table, "ID", "UserName", "Email", "Message", "Date");
                            var usersWithLastMessage = await GetUsersWithLastMessage();
                            foreach (var user in usersWithLastMessage)
                            {
                                table.AddCell(user.UserId.ToString());
                                table.AddCell(user.UserName);
                                table.AddCell(user.Email);
                                table.AddCell(user.LastMessageContent);
                                table.AddCell(user.LastMessageDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
                            }
                            break;

                        case "AttachmentsUsageReport":
                            table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth(); // 5 columns for AttachmentsUsageReport
                            AddTableHeaders(table, "AttachmentId", "UserName", "FileName", "DateUploaded", "RelatedMessageContent");
                            var attachmentReport = await GetAttachmentsUsageReport();
                            foreach (var attachment in attachmentReport)
                            {
                                table.AddCell(attachment.AttachmentId.ToString());
                                table.AddCell(attachment.UserName);
                                table.AddCell(attachment.FileName);
                                table.AddCell(attachment.DateUploaded.ToString("dd/MM/yyyy HH:mm"));
                                table.AddCell(attachment.RelatedMessageContent);
                            }
                            break;

                        case "ChatParticipationReports":
                            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth(); // 4 columns for ChatParticipationReports
                            AddTableHeaders(table, "ChatId", "ChatTitle", "UserName", "UserRole");
                            var chatParticipationReport = await GetChatParticipationReport();
                            foreach (var chat in chatParticipationReport)
                            {
                                table.AddCell(chat.ChatId.ToString());
                                table.AddCell(chat.ChatTitle);
                                table.AddCell(chat.UserName);
                                table.AddCell(chat.UserRole);
                            }
                            break;

                        case "MessageReportPrivateChats":
                            table = new Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth(); // 6 columns for MessageReportDto
                            AddTableHeaders(table, "MessageId", "SenderUserName", "ReceiverUsername", "MessageContent", "DateSent", "Status","ChatType");
                            var privateMessageReport = await GetMessageReport(ChatType.Private);
                            foreach (var message in privateMessageReport)
                            {
                                table.AddCell(message.MessageId.ToString());
                                table.AddCell(message.SenderUserName);
                                table.AddCell(message.ReceiverUserName);
                                table.AddCell(message.MessageContent);
                                table.AddCell(message.DateSent.ToString("dd/MM/yyyy HH:mm"));// Use european date format
                                table.AddCell(message.Status);
                                table.AddCell(message.ChatType);
                            }
                            break;
                        case "MessageReportGroupChats":
                            table = new Table(UnitValue.CreatePercentArray(6)).UseAllAvailableWidth(); // 6 columns for MessageReportDto
                            AddTableHeaders(table, "MessageId", "SenderUserName", "ReceiverUsername", "MessageContent", "DateSent", "Status", "ChatType");
                            var publicMessageReport = await GetMessageReport(ChatType.Group);
                            foreach (var message in publicMessageReport)
                            {
                                table.AddCell(message.MessageId.ToString());
                                table.AddCell(message.SenderUserName);
                                table.AddCell(message.ReceiverUserName);
                                table.AddCell(message.MessageContent);
                                table.AddCell(message.DateSent.ToString("dd/MM/yyyy HH:mm"));// Use european date format
                                table.AddCell(message.Status);
                                table.AddCell(message.ChatType);
                            }
                            break;

                        case "UserActivityReport": //
                            table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth(); // 5 columns for UserActivityReport
                            AddTableHeaders(table, "UserName", "LastLogin", "LastUpdated", "IsOnline", "TotalMessagesSent");
                            var userActivityReport = await GetUserActivities();
                            foreach (var user in userActivityReport)
                            {
                                table.AddCell(user.UserName);
                                table.AddCell(user.LastLogin?.ToString("dd/MM/yyyy HH:mm")); // Use european date format
                                table.AddCell(user.LastUpdated.ToString("dd/MM/yyyy HH:mm"));
                                table.AddCell(user.IsOnline ? "Online" : "Offline");
                                table.AddCell(user.TotalMessagesSent.ToString());
                            }
                            break;

                        default:
                            throw new ArgumentException("Invalid report type");
                    }

                    document.Add(table);
                }
                return memoryStream.ToArray();
            }
        }

        // To help style the table headers and format it consistently and nicely
        private void AddTableHeaders(Table table, params string[] headers)
        {
            foreach (var header in headers)
            {
                Cell cell = new Cell().Add(new Paragraph(header))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetBold();
                table.AddHeaderCell(cell);
            }
        }

    }
}
