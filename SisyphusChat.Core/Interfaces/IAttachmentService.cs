using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Interfaces
{
    public interface IAttachmentService : ICrud<Attachment>
    {
        Task<Attachment> GetByIdAsync(Guid id);
        Task<ICollection<Attachment>> GetAllByMessageIdAsync(string messageId);
    }
}