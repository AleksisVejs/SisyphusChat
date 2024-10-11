using SisyphusChat.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Services;
using OfficeOpenXml;

namespace SisyphusChat.Core.Services
{
    public class ReportService(IUnitOfWork unitOfWork): IReportService
    {
        public async Task<List<Attachment>> GetAttachments()
        {
            var attachments = await unitOfWork.AttachmentRepository.GetAllAsync();
            return attachments.ToList();
        }

        public async Task<List<Chat>> GetChats()
        {
            var Chats = await unitOfWork.ChatRepository.GetAllAsync();
            return Chats.ToList();
        }



        public async Task<List<Message>> GetMessages()
        {
            var Messages = await unitOfWork.MessageRepository.GetAllAsync();
            return Messages.ToList();
        }

        public async Task<List<User>> GetUsers()
        {
            var Users = await unitOfWork.UserRepository.GetAllAsync();
            return Users.ToList();
        }
        public async Task<MemoryStream> GenerateExcelAsync(string reportType)
        {
            MemoryStream memoryStream = new MemoryStream();

            using (ExcelPackage package = new ExcelPackage(memoryStream))
            {
                var worksheet = package.Workbook.Worksheets.Add(reportType);

                // Load data into the Excel sheet based on the report type
                switch (reportType)
                {
                    case "Attachments":
                        var attachments = await GetAttachments();
                        worksheet.Cells["A1"].LoadFromCollection(attachments, true);
                        break;
                    case "Chats":
                        var chats = await GetChats();
                        worksheet.Cells["A1"].LoadFromCollection(chats, true);
                        break;
                    case "Messages":
                        var messages = await GetMessages();
                        worksheet.Cells["A1"].LoadFromCollection(messages, true);
                        break;
                    case "Users":
                        var users = await GetUsers();
                        worksheet.Cells["A1"].LoadFromCollection(users, true);
                        break;
                    default:
                        throw new ArgumentException("Invalid report type");
                }

                
                worksheet.Cells.AutoFitColumns();
                package.Save();
            }

            memoryStream.Position = 0; 
            return memoryStream; 
        }

    }
}
