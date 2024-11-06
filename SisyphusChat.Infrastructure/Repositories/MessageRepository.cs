﻿using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace SisyphusChat.Infrastructure.Repositories;

public class MessageRepository(ApplicationDbContext context) : IMessageRepository
{
    public async Task AddAsync(Message entity)
    {
        entity.Status = MessageStatus.Sent;

        await context.Messages.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<Message>> GetAllAsync()
    {
        return await context.Messages.ToListAsync();
    }

    public async Task<Message> GetByIdAsync(string id)
    {
        var message = await context.Messages
            .FirstOrDefaultAsync(m => m.Id == id);

        if (message == null)
        {
            throw new EntityNotFoundException($"Message with ID {id} not found");
        }

        return message;
    }

    public async Task DeleteByIdAsync(string id)
    {
        var message = await context.Messages.FindAsync(id);

        if (message == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Messages.Remove(message);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Message entity)
    {
        if (entity == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }
        entity.LastUpdated = DateTime.Now;

        context.Messages.Update(entity);
        await context.SaveChangesAsync();
    }
}
