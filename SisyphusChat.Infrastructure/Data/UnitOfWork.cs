﻿using SisyphusChat.Core.Interfaces;
using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Interfaces;

namespace SisyphusChat.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(
        ApplicationDbContext context,
        IReportRepository reportRepository,
        IUserRepository userRepository,
        IMessageRepository messageRepository,
        IChatRepository chatRepository,
        IAttachmentRepository attachmentRepository)
    {
        _context = context;
        ReportRepository = reportRepository;
        UserRepository = userRepository;
        MessageRepository = messageRepository;
        ChatRepository = chatRepository;
        AttachmentRepository = attachmentRepository;
    }

    public IUserRepository UserRepository { get; }
    public IReportRepository ReportRepository { get; }

    public IMessageRepository MessageRepository { get; }

    public IChatRepository ChatRepository { get; }

    public IAttachmentRepository AttachmentRepository { get; }

    public Task SaveAsync()
    {
        return _context.SaveChangesAsync();
    }
}