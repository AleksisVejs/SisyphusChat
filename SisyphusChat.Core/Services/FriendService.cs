using AutoMapper;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Exceptions;

namespace SisyphusChat.Core.Services;

public class FriendService(IUnitOfWork unitOfWork, IMapper mapper) : IFriendService
{
    // Unused
    public async Task CreateAsync(FriendModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var friendEntity = mapper.Map<Friend>(model);

        await unitOfWork.FriendRepository.AddAsync(friendEntity);
        await unitOfWork.SaveAsync();
    }

    // Used only to accept friend requests
    public async Task UpdateAsync(FriendModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var friend = await unitOfWork.FriendRepository.GetByIdAsync(model.ReqSenderId.ToString() + ' ' + model.ReqReceiverId.ToString());
        friend.IsAccepted = true;

        await unitOfWork.FriendRepository.UpdateAsync(friend);
        await unitOfWork.SaveAsync();
    }

    // Unused
    public async Task<ICollection<FriendModel>> GetAllAsync()
    {
        var friendEntities = await unitOfWork.FriendRepository.GetAllAsync();

        return mapper.Map<ICollection<Friend>, ICollection<FriendModel>>(friendEntities);
    }

    public async Task<FriendModel> GetByIdAsync(string srid)
    {
        ArgumentException.ThrowIfNullOrEmpty(srid);

        var friendEntity = await unitOfWork.FriendRepository.GetByIdAsync(srid);

        return mapper.Map<FriendModel>(friendEntity);
    }

    public async Task DeleteByIdAsync(string srid)
    {
        ArgumentException.ThrowIfNullOrEmpty(srid);

        await unitOfWork.FriendRepository.DeleteByIdAsync(srid);
        await unitOfWork.SaveAsync();
    }

    public async Task<ICollection<UserModel>> GetAllFriendsAsync(string currentUserId)
    {
        ArgumentException.ThrowIfNullOrEmpty(currentUserId);

        var friendEntities = await unitOfWork.FriendRepository.GetAllFriendsAsync(currentUserId);

        return mapper.Map<ICollection<User>, ICollection<UserModel>>(friendEntities);
    }

    public async Task<ICollection<FriendModel>> GetAllSentRequestsAsync(string currentUserId)
    {
        ArgumentException.ThrowIfNullOrEmpty(currentUserId);

        var friendEntities = await unitOfWork.FriendRepository.GetAllSentRequestsAsync(currentUserId);

        return mapper.Map<ICollection<Friend>, ICollection<FriendModel>>(friendEntities);
    }

    public async Task<ICollection<FriendModel>> GetAllReceivedRequestsAsync(string currentUserId)
    {
        ArgumentException.ThrowIfNullOrEmpty(currentUserId);

        var friendEntities = await unitOfWork.FriendRepository.GetAllReceivedRequestsAsync(currentUserId);

        return mapper.Map<ICollection<Friend>, ICollection<FriendModel>>(friendEntities);
    }

    public async Task SendRequestAsync(string currentUserId, string receiverUserId)
    {
        Friend friendEntity;

        if (currentUserId != receiverUserId)
        {
            // Checks if friend request was already sent by other party
            try
            {
                friendEntity = await unitOfWork.FriendRepository.GetByIdAsync(receiverUserId + ' ' + currentUserId);
                if (friendEntity.IsAccepted)
                {
                    throw new InvalidOperationException("Already friends with user.");
                }
                // If not already friends with user, then accepts the request
                else
                {
                    friendEntity.IsAccepted = true;
                    await unitOfWork.FriendRepository.UpdateAsync(friendEntity);
                    await unitOfWork.SaveAsync();
                }
            }
            // Creates new friendship request if one wasn't found
            catch (EntityNotFoundException)
            {
                try
                {
                    friendEntity = await unitOfWork.FriendRepository.GetByIdAsync(currentUserId + ' ' + receiverUserId);
                    if (friendEntity.IsAccepted)
                    {
                        throw new InvalidOperationException("Already friends with user.");
                    }
                    else
                    {
                        throw new InvalidOperationException("Friendship request already sent.");
                    }
                }
                catch (EntityNotFoundException)
                {
                    friendEntity = new Friend
                    {
                        ReqSenderId = currentUserId,
                        ReqReceiverId = receiverUserId
                    };

                    await unitOfWork.FriendRepository.AddAsync(friendEntity);
                    await unitOfWork.SaveAsync();
                }
            }
        }
        else
        {
            throw new InvalidOperationException("Bo has no friends :(");
        }
        
    }
}
