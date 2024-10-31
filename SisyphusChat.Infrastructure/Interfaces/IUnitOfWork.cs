using SisyphusChat.Infrastructure.Interfaces;


namespace SisyphusChat.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IAttachmentRepository AttachmentRepository { get; } 
        
        IChatRepository ChatRepository { get; }

        IFriendRepository FriendRepository { get; }

        IMessageRepository MessageRepository { get; }

        IReportRepository ReportRepository { get; }

        IUserRepository UserRepository { get; }

        INotificationRepository NotificationRepository { get; }
        
        Task SaveAsync();
    }
}
