using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Infrastructure.Interfaces;

public interface IAttachmentRepository : IRepository<Attachment>
{
    Task<ICollection<Attachment>> GetAllByMessageIdAsync(string id);
}

