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

namespace SisyphusChat.Infrastructure.Repositories
{
    public class ReportRepository(ApplicationDbContext context) : IReportRepository
    {
        public async Task<List<Attachment>> GetAttachments() => await context.Attachments.ToListAsync();
        public async Task<List<Chat>> GetChats() => await context.Chats.ToListAsync();

        public async Task<List<Message>> GetMessages() => await context.Messages.ToListAsync();
        public async Task<List<User>> GetUsers() => await context.Users.ToListAsync();
 



    }
}
