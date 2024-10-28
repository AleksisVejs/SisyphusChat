using SisyphusChat.Infrastructure.Interfaces;


namespace SisyphusChat.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IAttachmentRepository AttachmentRepository { get; } 
        
        IChatRepository ChatRepository { get; }

        IFriendRepository FriendRepository { get; }

        IMessageRepository MessageRepository { get; }

        IAdminRepository AdminRepository { get; }

        IUserRepository UserRepository { get; }
        
        Task SaveAsync();
    }
}
