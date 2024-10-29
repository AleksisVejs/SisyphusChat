using SisyphusChat.Infrastructure.Interfaces;


namespace SisyphusChat.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IAdminRepository AdminRepository { get; }

        IAttachmentRepository AttachmentRepository { get; } 
        
        IChatRepository ChatRepository { get; }

        IFriendRepository FriendRepository { get; }

        IMessageRepository MessageRepository { get; }

        IReportRepository ReportRepository { get; }

        IUserRepository UserRepository { get; }
        
        Task SaveAsync();
    }
}
