﻿using AutoMapper;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Exceptions;

namespace SisyphusChat.Core.Services;

public class ChatService(IUnitOfWork unitOfWork, IMapper mapper) : IChatService
{
    public async Task CreateAsync(ChatModel model)
    {
        ArgumentNullException.ThrowIfNull(model);

        var chatEntity = mapper.Map<Chat>(model);

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

        try
        {
            chatEntity = await unitOfWork.ChatRepository.GetPrivateChatAsync(currentUserId, recipientUserId);
        }
        catch (EntityNotFoundException)
        {
            chatEntity = new Chat
            {
                Id = Guid.NewGuid(),
                Name = Guid.NewGuid().ToString(),
                Type = ChatType.Private,
                ChatUsers = new List<ChatUser>
                {
                    new() { UserId = currentUserId},
                    new() { UserId = recipientUserId}
                },
                Owner = unitOfWork.UserRepository.GetByIdAsync(currentUserId).Result
            };

            await unitOfWork.ChatRepository.AddAsync(chatEntity);
            await unitOfWork.SaveAsync();
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
}