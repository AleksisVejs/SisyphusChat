using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.DTO;
using SisyphusChat.Infrastructure.Entities;
using OfficeOpenXml.Style;
using OfficeOpenXml;

namespace SisyphusChat.Core.Services
{
    public class AdminService : IAdminService
    {
        // To inject the UnitOfWork dependency with readonly to ensure it is not changed
        private readonly IUnitOfWork _unitOfWork;

        // To inject the UnitOfWork dependency in the constructor
        public AdminService(IUnitOfWork unitOfWork)
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
            var chats = await _unitOfWork.AdminRepository.GetChats();
            return chats.ToList();
        }

        public async Task<List<Message>> GetMessages()
        {
            var messages = await _unitOfWork.AdminRepository.GetMessages();
            return messages.ToList();
        }

        public async Task<List<User>> GetUsers()
        {
            var users = await _unitOfWork.AdminRepository.GetUsers();
            return users.ToList();
        }

        public async Task<List<UserWithLastMessageDto>> GetUsersWithLastMessage()
        {
            return await _unitOfWork.AdminRepository.GetUsersWithLastMessage();
        }

        public async Task<List<AttachmentUsageReportDto>> GetAttachmentsUsageReport()
        {
            return await _unitOfWork.AdminRepository.GetAttachmentsUsageReport();
        }

        public async Task<List<MessageReportDto>> GetMessageReport(ChatType chatType)
        {
            return await _unitOfWork.AdminRepository.GetMessagesReport(chatType);
        }

        public async Task<List<UserActivityReportDto>> GetUserActivities()
        {
            return await _unitOfWork.AdminRepository.GetUsersActivityReport();
        }

        public async Task<List<ChatParticipationReportDto>> GetChatParticipationReport()
        {
            return await _unitOfWork.AdminRepository.GetChatParticipationReports();
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
                                table.AddCell(attachment.TimeCreated.ToString("dd/MM/yyyy HH:mm")); 
                            }
                            break;

                        case "Chats":
                            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth();
                            var chats = await GetChats();
                            AddTableHeaders(table, "ID", "Title", "DateCreated");
                            foreach (var chat in chats)
                            {
                                table.AddCell(chat.Id.ToString());
                                table.AddCell(chat.Name);
                                table.AddCell(chat.TimeCreated.ToString("dd/MM/yyyy HH:mm"));
                            }
                            break;

                        case "Messages":
                            table = new Table(UnitValue.CreatePercentArray(3)).UseAllAvailableWidth(); // 3 columns for Messages
                            var messages = await GetMessages();
                            AddTableHeaders(table, "ID", "Content", "DateSent");
                            foreach (var message in messages)
                            {
                                table.AddCell(message.Id.ToString());
                                var shortContent = message.Content.Length > 60 ? message.Content.Substring(0, 40) + "..." : message.Content;
                                table.AddCell(shortContent);
                                table.AddCell(message.TimeCreated.ToString("dd/MM/yyyy HH:mm"));
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

                        case "Users With Last Message":
                            table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth(); // 5 columns for UsersWithLastMessage
                            AddTableHeaders(table, "ID", "UserName", "Email", "Message", "Date");
                            var usersWithLastMessage = await GetUsersWithLastMessage();
                            foreach (var user in usersWithLastMessage)
                            {
                                table.AddCell(user.UserId.ToString());
                                table.AddCell(user.UserName);
                                table.AddCell(user.Email);
                                var lastMessage = user.LastMessageContent.Length > 60 ? user.LastMessageContent.Substring(0, 40) + "..." : user.LastMessageContent;
                                table.AddCell(lastMessage);
                                table.AddCell(user.LastMessageDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A");
                            }
                            break;

                        case "Attachments Usage":
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

                        case "Chat Participation":
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

                        case "Message Private Chats Activity":
                            table = new Table(UnitValue.CreatePercentArray(4)).UseAllAvailableWidth(); // 6 columns for MessageReportDto
                            AddTableHeaders(table, "MessageId", "SenderUserName", "MessageContent", "DateSent");
                            var privateMessageReport = await GetMessageReport(ChatType.Private);
                            foreach (var message in privateMessageReport)
                            {
                                table.AddCell(message.MessageId.ToString());
                                table.AddCell(message.SenderUserName);
                                var shortmessage = message.MessageContent.Length > 60 ? message.MessageContent.Substring(0, 40) + "..." : message.MessageContent;
                                table.AddCell(shortmessage);
                                table.AddCell(message.DateSent.ToString("dd/MM/yyyy HH:mm"));// Use european date format                            }
                            }break;
                        case "Message Group Chats Activity":
                            table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth(); // 6 columns for MessageReportDto
                            AddTableHeaders(table, "MessageId", "SenderUserName", "MessageContent", "DateSent", "Status");
                            var publicMessageReport = await GetMessageReport(ChatType.Group);
                            foreach (var message in publicMessageReport)
                            {
                                table.AddCell(message.MessageId.ToString());
                                table.AddCell(message.SenderUserName);
                                var shortmessage = message.MessageContent.Length > 60 ? message.MessageContent.Substring(0, 40) + "..." : message.MessageContent;
                                table.AddCell(shortmessage);
                                table.AddCell(message.DateSent.ToString("dd/MM/yyyy HH:mm"));// Use european date format
                            }
                            break;

                        case "User Activity": //
                            table = new Table(UnitValue.CreatePercentArray(5)).UseAllAvailableWidth(); // 5 columns for UserActivityReport
                            AddTableHeaders(table, "UserName", "LastLogin", "LastUpdated", "IsOnline", "TotalMessagesSent");
                            var userActivityReport = await GetUserActivities();
                            foreach (var user in userActivityReport)
                            {
                                table.AddCell(user.UserName);
                                table.AddCell(user.LastLogin?.ToString("dd/MM/yyyy HH:mm") ?? "N/A"); // Something is off with this line
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

        public async Task<byte[]> GenerateExcelAsync(string reportType)
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add($"{reportType} Report");

                worksheet.Cells["A1"].Value = $"{reportType} Report";
                worksheet.Cells["A1"].Style.Font.Size = 18;
                worksheet.Cells["A1"].Style.Font.Bold = true;
                worksheet.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                int row = 3;

                switch (reportType)
                {
                    case "Attachments":
                        worksheet.Cells[row, 1].Value = "ID";
                        worksheet.Cells[row, 2].Value = "FileName";
                        worksheet.Cells[row, 3].Value = "DateUploaded";
                        var attachments = await GetAttachments();
                        foreach (var attachment in attachments)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = attachment.Id;
                            worksheet.Cells[row, 2].Value = attachment.FileName;
                            worksheet.Cells[row, 3].Value = attachment.TimeCreated.ToString("dd/MM/yyyy HH:mm");
                        }
                        break;

                    case "Chats":
                        worksheet.Cells[row, 1].Value = "ID";
                        worksheet.Cells[row, 2].Value = "Title";
                        worksheet.Cells[row, 3].Value = "DateCreated";
                        var chats = await GetChats();
                        foreach (var chat in chats)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = chat.Id;
                            worksheet.Cells[row, 2].Value = chat.Name;
                            worksheet.Cells[row, 3].Value = chat.TimeCreated.ToString("dd/MM/yyyy HH:mm");
                        }
                        break;

                    case "Messages":
                        worksheet.Cells[row, 1].Value = "ID";
                        worksheet.Cells[row, 2].Value = "Content";
                        worksheet.Cells[row, 3].Value = "DateSent";
                        var messages = await GetMessages();
                        foreach (var message in messages)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = message.Id;
                            worksheet.Cells[row, 2].Value = message.Content;
                            worksheet.Cells[row, 3].Value = message.LastUpdated.ToString("dd/MM/yyyy HH:mm");
                        }
                        break;

                    case "Users":
                        worksheet.Cells[row, 1].Value = "ID";
                        worksheet.Cells[row, 2].Value = "UserName";
                        worksheet.Cells[row, 3].Value = "Email";
                        var users = await GetUsers();
                        foreach (var user in users)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = user.Id;
                            worksheet.Cells[row, 2].Value = user.UserName;
                            worksheet.Cells[row, 3].Value = user.Email;
                        }
                        break;

                    case "Users With Last Message":
                        worksheet.Cells[row, 1].Value = "ID";
                        worksheet.Cells[row, 2].Value = "UserName";
                        worksheet.Cells[row, 3].Value = "Email";
                        worksheet.Cells[row, 4].Value = "Message";
                        worksheet.Cells[row, 5].Value = "Date";
                        var usersWithLastMessage = await GetUsersWithLastMessage();
                        foreach (var user in usersWithLastMessage)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = user.UserId;
                            worksheet.Cells[row, 2].Value = user.UserName;
                            worksheet.Cells[row, 3].Value = user.Email;
                            worksheet.Cells[row, 4].Value = user.LastMessageContent;
                            worksheet.Cells[row, 5].Value = user.LastMessageDate?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                        }
                        break;

                    case "Attachments Usage":
                        worksheet.Cells[row, 1].Value = "AttachmentId";
                        worksheet.Cells[row, 2].Value = "UserName";
                        worksheet.Cells[row, 3].Value = "FileName";
                        worksheet.Cells[row, 4].Value = "DateUploaded";
                        worksheet.Cells[row, 5].Value = "RelatedMessageContent";
                        var attachmentUsageReport = await GetAttachmentsUsageReport();
                        foreach (var attachment in attachmentUsageReport)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = attachment.AttachmentId;
                            worksheet.Cells[row, 2].Value = attachment.UserName;
                            worksheet.Cells[row, 3].Value = attachment.FileName;
                            worksheet.Cells[row, 4].Value = attachment.DateUploaded.ToString("dd/MM/yyyy HH:mm");
                            worksheet.Cells[row, 5].Value = attachment.RelatedMessageContent;
                        }
                        break;

                    case "Chat Participation":
                        worksheet.Cells[row, 1].Value = "ChatId";
                        worksheet.Cells[row, 2].Value = "ChatTitle";
                        worksheet.Cells[row, 3].Value = "UserName";
                        worksheet.Cells[row, 4].Value = "UserRole";
                        var chatParticipationReport = await GetChatParticipationReport();
                        foreach (var chat in chatParticipationReport)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = chat.ChatId;
                            worksheet.Cells[row, 2].Value = chat.ChatTitle;
                            worksheet.Cells[row, 3].Value = chat.UserName;
                            worksheet.Cells[row, 4].Value = chat.UserRole;
                        }
                        break;

                    case "User Activity":
                        worksheet.Cells[row, 1].Value = "UserName";
                        worksheet.Cells[row, 2].Value = "LastLogin";
                        worksheet.Cells[row, 3].Value = "LastUpdated";
                        worksheet.Cells[row, 4].Value = "IsOnline";
                        worksheet.Cells[row, 5].Value = "TotalMessagesSent";
                        var userActivityReport = await GetUserActivities();
                        foreach (var user in userActivityReport)
                        {
                            row++;
                            worksheet.Cells[row, 1].Value = user.UserName;
                            worksheet.Cells[row, 2].Value = user.LastLogin?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";
                            worksheet.Cells[row, 3].Value = user.LastUpdated.ToString("dd/MM/yyyy HH:mm");
                            worksheet.Cells[row, 4].Value = user.IsOnline ? "Online" : "Offline";
                            worksheet.Cells[row, 5].Value = user.TotalMessagesSent;
                        }
                        break;

                    default:
                        throw new ArgumentException("Invalid report type");
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray(); // Return the byte array to be used for downloading
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
