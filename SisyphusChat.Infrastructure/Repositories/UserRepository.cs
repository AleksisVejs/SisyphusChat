﻿
/*using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Exceptions;
using SisyphusChat.Infrastructure.Interfaces;

namespace SisyphusChat.Infrastructure.Repositories;

public class UserRepository(ApplicationDbContext context) : IUserRepository
{
    public async Task AddAsync(User entity)
    {
        await context.Users.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<User>> GetAllAsync()
    {
        return await context.Users.Include(u => u.Chats).ThenInclude(c => c.Members).ToListAsync();
    }

    public async Task<User> GetByIdAsync(string id)
    {
        var user = await context.Users
            .Include(u => u.Chats)
            .FirstOrDefaultAsync(g => g.Id == id);

        if (user == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        return user;
    }

    public async Task DeleteByIdAsync(string id)
    {
        var user = await context.Users.FindAsync(id);

        if (user == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User entity)
    {
        if (entity == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Users.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task<User> GetUserByChatIdAsync(string chatId)
    {
        var user = await context.Users
            .Where(u => u.Chats.Any(c => c.Id == chatId))
            .FirstOrDefaultAsync();

        if (user == null)
        {
            throw new EntityNotFoundException($"User with chat id: {chatId} is not found");
        }

        return user;
    }
}
*/