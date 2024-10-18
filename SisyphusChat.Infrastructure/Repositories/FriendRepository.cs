using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SisyphusChat.Infrastructure.Repositories;

public class FriendRepository(ApplicationDbContext context) : IFriendRepository
{
    public async Task AddAsync(Friend entity)
    {
        entity.IsAccepted = false;

        await context.Friends.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<Friend>> GetAllAsync()
    {
        var friends = await context.Friends
            .ToListAsync();

        return friends;
    }

    public async Task<Friend> GetByIdAsync(string srid)
    {
        // Splits sender and receiver id in two
        string[] ids = srid.Split(' ');

        var friend = await context.Friends
            .Include(f => f.IsAccepted)
            .FirstOrDefaultAsync(f => f.ReqSenderId.ToString() == ids[0] && f.ReqSenderId.ToString() == ids[1]);

        if (friend == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        return friend;
    }

    public async Task DeleteByIdAsync(string srid)
    {
        // Splits sender and receiver id in two
        string[] ids = srid.Split(' ');

        var friend = await context.Friends.FindAsync(ids[0], ids[1]);

        if (friend == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Friends.Remove(friend);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Friend entity)
    {
        if (entity == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Friends.Update(entity);
        await context.SaveChangesAsync();
    }
}
