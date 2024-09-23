/*using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SisyphusChat.Infrastructure.Repositories;

public class ChatRepository(ApplicationDbContext context) : IChatRepository
{
    public async Task AddAsync(Chat entity)
    {
        entity.ID = Guid.NewGuid(); // Assuming Id is of type Guid in BaseEntity
        entity.TimeCreated = DateTime.Now;
        entity.IsReported = false; // Assuming a default value for IsReported

        await context.Chats.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<Chat>> GetAllAsync()
    {
        var chats = await context.Chats
            .ToListAsync();

        return chats;
    }

    public async Task<Chat> GetByIdAsync(string id)
    {
        var chat = await context.Chats
            .Include(c => c.Members)
            .ThenInclude(m => m.User)
            .Include(c => c.Owner)
            .ThenInclude(o => o.Owner)
            .Include(c => c.Messages
            .OrderBy(m => m.SentAt))
            .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (chat == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        return chat;
    }

    public async Task DeleteByIdAsync(string id)
    {
        var chat = await context.Chats.FindAsync(id);

        if (chat == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Chats.Remove(chat);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Chat entity)
    {
        if (entity == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Chats.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task<Chat> GetPrivateChatAsync(string currentUserId, string recipientUserId)
    {
        var chat = await context.Chats
            .Include(c => c.Members)
            .FirstOrDefaultAsync(c => c.Type == ChatType.Private &&
                                      c.Members.Any(m => m.UserId == currentUserId) &&
                                      c.Members.Any(m => m.UserId == recipientUserId));

        if (chat == null)
        {
            throw new EntityNotFoundException($"Chat with current user id: {currentUserId} and recipient user id: {recipientUserId} is not found");
        }

        return chat;
    }
}
*/