using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Data;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace SisyphusChat.Core.Services;

public class UserService(
    IUnitOfWork unitOfWork,
    UserManager<User> userManager,
    IHttpContextAccessor httpContextAccessor,
    IMapper mapper)
    : IUserService
{
    private readonly ClaimsPrincipal _principal = httpContextAccessor.HttpContext.User;

    // CRUD methods

    public async Task CreateAsync(UserModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var userEntity = mapper.Map<User>(model);

        await unitOfWork.UserRepository.AddAsync(userEntity);
        await unitOfWork.SaveAsync();
    }

    public async Task UpdateAsync(UserModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var userEntity = mapper.Map<User>(model);

        await unitOfWork.UserRepository.UpdateAsync(userEntity);
        await unitOfWork.SaveAsync();
    }

    public async Task<ICollection<UserModel>> GetAllAsync()
    {
        var userEntities = await unitOfWork.UserRepository.GetAllAsync();

        return mapper.Map<ICollection<User>, ICollection<UserModel>>(userEntities);
    }

    public async Task<UserModel> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var userEntity = await unitOfWork.UserRepository.GetByIdAsync(id);

        return mapper.Map<UserModel>(userEntity);
    }

    public async Task<UserModel> GetByUsernameAsync(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return null; // Return null if username is empty or null
        }

        // Get the current user using the existing method
        var currentUser = await GetCurrentContextUserAsync();

        // Check if the username is the same as the current user's username
        if (currentUser != null && currentUser.UserName == userName)
        {
            return null; // Return null if trying to get the current user himself
        }

        var userEntity = await unitOfWork.UserRepository.GetByUsernameAsync(userName);
        return userEntity == null ? null : mapper.Map<UserModel>(userEntity); // Return null if user not found
    }




    public async Task DeleteByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        await unitOfWork.UserRepository.DeleteByIdAsync(id);
        await unitOfWork.SaveAsync();
    }

    // Service methods

    public async Task<UserModel> GetCurrentContextUserAsync()
    {
        var userEntity = await userManager.GetUserAsync(_principal);

        if (userEntity == null)
        {
            throw new InvalidOperationException("The current user could not be found");
        }

        return mapper.Map<UserModel>(userEntity);
    }

    public async Task<ICollection<UserModel>> GetAllUsersAsync()
    {
        var users = await unitOfWork.UserRepository.GetAllAsync();
        return mapper.Map<ICollection<UserModel>>(users);
    }

    public async Task<ICollection<ChatUserModel>> GetUsersByUserNamesAsync(string[] userNamesArray)
    {
        var userEntities = await unitOfWork.UserRepository.GetAllAsync();
        var users = userEntities
            .Where(x => userNamesArray.Contains(x.UserName))
            .Select(u => new ChatUserModel
            {
                UserId = u.Id
            })
            .ToList();

        return users;
    }

    public async Task<ICollection<UserModel>> GetAllExceptCurrentUserAsync(UserModel currentUser)
    {
        var userEntities = await unitOfWork.UserRepository.GetAllAsync();
        var users = userEntities
            .Where(u => u.Id != currentUser.Id)
            .ToList();

        return mapper.Map<ICollection<User>, ICollection<UserModel>>(users);
    }

    public async Task SetUserOnlineAsync(string userId)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.IsOnline = true;
            user.LastUpdated = DateTime.UtcNow;
            await unitOfWork.SaveAsync();
        }
    }

    public async Task SetUserOfflineAsync(string userId)
    {
        var user = await unitOfWork.UserRepository.GetByIdAsync(userId);
        if (user != null)
        {
            user.IsOnline = false;
            await unitOfWork.SaveAsync();
        }
    }
}