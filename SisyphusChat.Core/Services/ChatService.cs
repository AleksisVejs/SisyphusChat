using AutoMapper;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Exceptions;

namespace SisyphusChat.Core.Services;

public class ChatService(IUnitOfWork unitOfWork, IMapper mapper) : IChatService
{
    public async Task CreateAsync(ChatModel model)
    {
        var chatEntity = new Chat
        {
            Id = model.Id == null ? Guid.NewGuid().ToString() : new Guid(model.Id).ToString(),
            Name = model.Name,
            Type = model.Type,
            OwnerId = model.OwnerId,
            IsReported = model.IsReported,
            TimeCreated = model.TimeCreated,
            LastUpdated = model.LastUpdated,
            ChatUsers = new List<ChatUser>()
        };

        // Step 1: Add owner to ChatUsers if the owner exists
        var owner = await unitOfWork.UserRepository.GetByIdAsync(model.OwnerId);
        if (owner == null)
        {
            throw new InvalidOperationException("Owner not found");
        }

        chatEntity.ChatUsers.Add(new ChatUser
        {
            ChatId = chatEntity.Id,
            UserId = owner.Id,
            User = owner
        });

        // Step 2: Add other users from model.ChatUsers
        if (model.ChatUsers != null && model.ChatUsers.Any())
        {
            foreach (var userModel in model.ChatUsers)
            {
                Console.WriteLine($"Attempting to add UserId: {userModel.UserId} to ChatUsers");

                var user = await unitOfWork.UserRepository.GetByIdAsync(userModel.UserId);

                if (user == null)
                {
                    // Log or handle the missing user case
                    Console.WriteLine($"User with ID {userModel.UserId} not found in database. Skipping.");
                    continue; // Skip this user if they don't exist in the database
                }

                chatEntity.ChatUsers.Add(new ChatUser
                {
                    ChatId = chatEntity.Id,
                    UserId = user.Id,
                    User = user
                });
            }
        }


        await unitOfWork.ChatRepository.AddAsync(chatEntity);
        await unitOfWork.SaveAsync();
    }



    public async Task UpdateAsync(ChatModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var chat = await unitOfWork.ChatRepository.GetByIdAsync(model.Id.ToString());

        await unitOfWork.ChatRepository.UpdateAsync(chat);
        await unitOfWork.SaveAsync();
    }

    public async Task DeleteUserFromChatByIdAsync(string userId, string chatId)
    {
        var chat = await unitOfWork.ChatRepository.GetByIdAsync(chatId);
        var userToRemove = chat.ChatUsers.FirstOrDefault(x => x.UserId == userId);

        if (userToRemove != null)
        {
            chat.ChatUsers.Remove(userToRemove);
        }

        await unitOfWork.ChatRepository.UpdateAsync(chat);
        await unitOfWork.SaveAsync();
    }

    public async Task UpdateWithMembersAsync(ChatModel model, ICollection<string> newMembers)
    {
        ArgumentNullException.ThrowIfNull(model);

        var chat = await unitOfWork.ChatRepository.GetByIdAsync(model.Id.ToString());

        chat.Name = model.Name;

        var existingMemberIds = chat.ChatUsers.Select(cm => cm.UserId).ToHashSet();

        var dbUsers = await unitOfWork.UserRepository.GetAllAsync();
        var ids = dbUsers
            .Where(u => u.UserName != null && newMembers.Contains(u.UserName))
            .Select(u => u.Id);

        foreach (var memberId in ids)
        {
            if (!existingMemberIds.Contains(memberId))
            {
                chat.ChatUsers.Add(new ChatUser { UserId = memberId });
            }
        }

        await unitOfWork.ChatRepository.UpdateAsync(chat);
        await unitOfWork.SaveAsync();
    }

    public async Task<ICollection<ChatModel>> GetAllAsync()
    {
        var chatEntities = await unitOfWork.ChatRepository.GetAllAsync();

        return mapper.Map<ICollection<Chat>, ICollection<ChatModel>>(chatEntities);
    }

    public async Task<ChatModel> GetByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        var chatEntity = await unitOfWork.ChatRepository.GetByIdAsync(id);

        // Ensure messages are ordered and status is loaded
        chatEntity.Messages = chatEntity.Messages
            .OrderBy(m => m.TimeCreated)
            .ToList();

        return mapper.Map<ChatModel>(chatEntity);
    }

    public async Task DeleteByIdAsync(string id)
    {
        ArgumentException.ThrowIfNullOrEmpty(id);

        await unitOfWork.ChatRepository.DeleteByIdAsync(id);
        await unitOfWork.SaveAsync();
    }

    public async Task<ChatModel> OpenOrCreatePrivateChatAsync(string currentUserId, string recipientUserId)
    {
        Chat chatEntity;

        if (recipientUserId == null)
        {
            throw new EntityNotFoundException($"User with ID {recipientUserId} not found");
        }

        // checking if the user is trying to create a chat with themselves
        if (currentUserId == recipientUserId)
        {
            try
            {
                chatEntity = await unitOfWork.ChatRepository.GetSelfChatAsync(currentUserId);
            }
            catch (EntityNotFoundException)
            {
                var currentUser = await unitOfWork.UserRepository.GetByIdAsync(currentUserId);

                chatEntity = new Chat
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"{currentUser.UserName}",
                    Type = ChatType.Private,
                    ChatUsers = new List<ChatUser>
                    {
                        new() { UserId = currentUserId }
                    },
                    Owner = currentUser
                };

                await unitOfWork.ChatRepository.AddAsync(chatEntity);
                await unitOfWork.SaveAsync();
            }
        }
        else
        {
            try
            {
                chatEntity = await unitOfWork.ChatRepository.GetPrivateChatAsync(currentUserId, recipientUserId);
            }
            catch (EntityNotFoundException)
            {
                var currentUser = await unitOfWork.UserRepository.GetByIdAsync(currentUserId);
                var recipientUser = await unitOfWork.UserRepository.GetByIdAsync(recipientUserId);

                chatEntity = new Chat
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = $"{currentUser.UserName}, {recipientUser.UserName}",
                    Type = ChatType.Private,
                    ChatUsers = new List<ChatUser>
                {
                    new() { UserId = currentUserId },
                    new() { UserId = recipientUserId }
                },
                    Owner = await unitOfWork.UserRepository.GetByIdAsync(currentUserId)
                };

                await unitOfWork.ChatRepository.AddAsync(chatEntity);
                await unitOfWork.SaveAsync();
            }
        }

        var user = await unitOfWork.UserRepository.GetByIdAsync(currentUserId);
        var chatUser = chatEntity.ChatUsers.FirstOrDefault(u => u.UserId == currentUserId);

        if (chatUser == null)
        {
            chatEntity.ChatUsers.Add(new ChatUser
            {
                ChatId = chatEntity.Id,
                Chat = chatEntity,
                UserId = user.Id,
                User = user
            });
        }

        await unitOfWork.ChatRepository.UpdateAsync(chatEntity);
        await unitOfWork.SaveAsync();
        await unitOfWork.UserRepository.UpdateAsync(user);
        await unitOfWork.SaveAsync();

        return mapper.Map<ChatModel>(chatEntity);
    }


    public async Task<ICollection<ChatModel>> GetAssociatedChatsAsync(UserModel currentUser)
    {
        var chats = await unitOfWork.ChatRepository.GetAllAsync();

        var associatedChats = chats
            .Where(c => c.ChatUsers.Any(m => m.UserId == currentUser.Id && c.Type == ChatType.Group)).ToList();

        return mapper.Map<ICollection<Chat>, ICollection<ChatModel>>(associatedChats);
    }

    public async Task<ICollection<UserModel>> GetUsersNotInChatAsync(string chatId)
    {
        var chat = await unitOfWork.ChatRepository.GetByIdAsync(chatId);
        var existingMemberIds = chat.ChatUsers.Select(cm => cm.UserId).ToHashSet();

        var dbUsers = await unitOfWork.UserRepository.GetAllAsync();
        var availableUsers = dbUsers.Where(u => 
            !existingMemberIds.Contains(u.Id) && 
            !u.IsDeleted && // Filter out deleted users
            u.UserName != "DELETED_USER" // Filter out system deleted user
        );

        return mapper.Map<ICollection<User>, ICollection<UserModel>>(availableUsers.ToList());
    }
}