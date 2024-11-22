using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Models;

namespace SisyphusChat.Core.Interfaces
{
    public interface IAttachmentService : ICrud<AttachmentModel>
    {
        Task<AttachmentModel> GetByIdAsync(string id);
        Task<ICollection<AttachmentModel>> GetAllByMessageIdAsync(string messageId);
    }
}