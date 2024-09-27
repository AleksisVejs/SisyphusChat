using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Infrastructure.Interfaces
{
    public interface IAttachmentRepository
    {
        Task<Attachment> GetByIdAsync(Guid id);
        Task AddAsync(Attachment attachment);
        Task UpdateAsync(Attachment attachment);
        Task DeleteAsync(Guid id);
        Task<ICollection<Attachment>> GetAllByMessageIdAsync(string messageId);
    }
}
