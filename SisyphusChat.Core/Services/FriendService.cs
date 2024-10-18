using AutoMapper;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Exceptions;

namespace SisyphusChat.Core.Services;

public class FriendService(IUnitOfWork unitOfWork, IMapper mapper) : IFriendService
{
    public async Task CreateAsync(FriendModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var friendEntity = mapper.Map<Friend>(model);

        await unitOfWork.FriendRepository.AddAsync(friendEntity);
        await unitOfWork.SaveAsync();
    }

    public async Task UpdateAsync(FriendModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var friend = await unitOfWork.FriendRepository.GetByIdAsync(model.ReqSenderId.ToString() + ' ' + model.ReqReceiverId.ToString());

        await unitOfWork.FriendRepository.UpdateAsync(friend);
        await unitOfWork.SaveAsync();
    }

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
}
