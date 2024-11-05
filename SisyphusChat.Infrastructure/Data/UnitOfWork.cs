﻿using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace SisyphusChat.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        IAdminRepository adminRepository,
        IAttachmentRepository attachmentRepository,
        IChatRepository chatRepository,
        IFriendRepository friendRepository,
        IMessageRepository messageRepository,
        IReportRepository reportRepository,
        IUserRepository userRepository,
        INotificationRepository notificationRepository
        )
    {
        _context = context;
        AdminRepository = adminRepository;
        AttachmentRepository = attachmentRepository;
        ChatRepository = chatRepository;
        FriendRepository = friendRepository;
        MessageRepository = messageRepository;
        ReportRepository = reportRepository;
        UserRepository = userRepository;
        NotificationRepository = notificationRepository;
    }

    public IAdminRepository AdminRepository { get; }

    public IAttachmentRepository AttachmentRepository { get; }

    public IChatRepository ChatRepository { get; }

    public IFriendRepository FriendRepository { get; }

    public IMessageRepository MessageRepository { get; }

    public IReportRepository ReportRepository { get; }

    public IUserRepository UserRepository { get; }
    public INotificationRepository NotificationRepository { get; }


    public Task SaveAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}