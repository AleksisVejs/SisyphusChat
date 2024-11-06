using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SisyphusChat.Infrastructure.Repositories;

public class ChatRepository(ApplicationDbContext context) : IChatRepository
{
    public async Task AddAsync(Chat entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        entity.TimeCreated = DateTime.Now;
        entity.IsReported = false;

        // Attach users to prevent tracking issues
        foreach (var chatUser in entity.ChatUsers)
        {
            // Attach the user to the context, which ensures it's not re-tracked
            if (chatUser.User != null)
            {
                context.Attach(chatUser.User);
            }
        }

        // Add the chat entity
        await context.Chats.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<Chat>> GetAllAsync()
    {
        var chats = await context.Chats
            .Include(chat => chat.ChatUsers)
            .ToListAsync();

        return chats;
    }

    public async Task<Chat> GetByIdAsync(string id)
    {
        return await context.Chats
            .Include(c => c.ChatUsers)
                .ThenInclude(cu => cu.User)
            .Include(c => c.Messages
                .OrderBy(m => m.TimeCreated))
                .ThenInclude(m => m.Sender)
            .FirstOrDefaultAsync(c => c.Id == id)
            ?? throw new EntityNotFoundException($"Chat with ID {id} not found");
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
            .Include(c => c.ChatUsers)
            .FirstOrDefaultAsync(c => c.Type == ChatType.Private &&
                                      c.ChatUsers.Any(m => m.UserId.ToString() == currentUserId) &&
                                      c.ChatUsers.Any(m => m.UserId.ToString() == recipientUserId));

        if (chat == null)
        {
            throw new EntityNotFoundException($"Chat with current user id: {currentUserId} and recipient user id: {recipientUserId} is not found");
        }

        return chat;
    }

    public async Task<Chat> GetSelfChatAsync(string userId)
    {
        var chat = await context.Chats
            .Include(c => c.ChatUsers)
            .FirstOrDefaultAsync(c => c.Type == ChatType.Private &&
                                      c.ChatUsers.Count == 1 &&
                                      c.ChatUsers.Any(m => m.UserId.ToString() == userId));

        if (chat == null)
        {
            throw new EntityNotFoundException($"Self-chat for user id: {userId} is not found");
        }

        return chat;
    }
}
