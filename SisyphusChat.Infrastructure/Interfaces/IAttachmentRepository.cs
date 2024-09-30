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
        Task<Attachment> GetByIdAsync(string id);
        Task AddAsync(Attachment attachment);
        Task<ICollection<Attachment>> GetAllAsync();
        Task UpdateAsync(Attachment attachment);
        Task DeleteByIdAsync(string id);
        Task<ICollection<Attachment>> GetAllByMessageIdAsync(string id);
    }
}
