using SisyphusChat.Core.Models;


namespace SisyphusChat.Core.Interfaces
{
    public interface IAttachmentService : ICrud<AttachmentModel>
    {
        Task<AttachmentModel> GetByIdAsync(string id);
        Task<ICollection<AttachmentModel>> GetAllByMessageIdAsync(string messageId);
    }
}