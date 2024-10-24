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
            .Include(u => u.ReqSender)
            .Include(u => u.ReqReceiver)
            .FirstOrDefaultAsync(f => f.ReqSenderId == ids[0] && f.ReqReceiverId == ids[1]);

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

    public async Task<ICollection<User>> GetAllFriendsAsync(string currentUserId)
    {
        // Fetch friendships that are accepted for the current user
        var friendships = await context.Friends
            .Where(u => (u.ReqSenderId == currentUserId || u.ReqReceiverId == currentUserId) && u.IsAccepted)
            .Select(f => f.ReqSenderId == currentUserId ? f.ReqReceiverId : f.ReqSenderId) // Get the friend's ID
            .ToListAsync();

        // Fetch the users who are in the friendships
        var friends = await context.Users
            .Where(u => friendships.Contains(u.Id)) // Use Contains for filtering
            .ToListAsync();

        return friends;
    }

    public async Task<ICollection<Friend>> GetAllSentRequestsAsync(string currentUserId)
    {
        var friends = await context.Friends
            .Include(f => f.ReqReceiver)
            .Where(u => u.ReqSenderId == currentUserId && u.IsAccepted == false)
            .ToListAsync();

        return friends;
    }

    public async Task<ICollection<Friend>> GetAllReceivedRequestsAsync(string currentUserId)
    {
        var friends = await context.Friends
            .Include(fr => fr.ReqSender)
            .Where(u => u.ReqReceiverId == currentUserId && u.IsAccepted == false)
            .ToListAsync();

        return friends;
    }
}
