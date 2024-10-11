using SisyphusChat.Infrastructure.Interfaces;


namespace SisyphusChat.Core.Interfaces
{
    public interface IUnitOfWork
    {
        IReportRepository ReportRepository { get; }
        IUserRepository UserRepository { get; }

        IMessageRepository MessageRepository { get; }

        IChatRepository ChatRepository { get; }

        IAttachmentRepository AttachmentRepository { get; } 

        Task SaveAsync();
    }
}
