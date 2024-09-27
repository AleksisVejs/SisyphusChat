using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;

namespace SisyphusChat.Infrastructure.Repositories
{
    public class AttachmentRepository : IAttachmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AttachmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Attachment> GetByIdAsync(Guid id)
        {
            return await _context.Attachments.FindAsync(id);
        }

        public async Task AddAsync(Attachment attachment)
        {
            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attachment attachment)
        {
            _context.Attachments.Update(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ICollection<Attachment>> GetAllByMessageIdAsync(string messageId)
        {
            return await _context.Attachments
                .Where(a => a.MessageID.ToString() == messageId)
                .ToListAsync();
        }
    }
}
