using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using Microsoft.Extensions.Logging;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Services;

namespace SisyphusChat.Web.Hubs;

public class ChatHub(
    IMessageService messageService,
    IUserService userService,
    IChatService chatService,
    IHubContext<NotificationHub> notificationHubContext,
    INotificationService notificationService,
    ILogger<ChatHub> logger,
    IFriendService friendService
    ) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("⭐ Client connecting to ChatHub");
        await userService.GetCurrentContextUserAsync();
        await base.OnConnectedAsync();
    }

    public async Task JoinChat(string chatId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task LeaveChat(string chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatId);
    }

    public async Task SendMessage(string user, string message, string chatId)
    {
        var chat = await chatService.GetByIdAsync(chatId);
        var chatMembersUserNames = chat.ChatUsers.Select(x => x.User.UserName).ToList();
        var isPartOfTheChat = chatMembersUserNames.Contains(user);

        if (!isPartOfTheChat)
        {
            await Clients.Caller.SendAsync("ReceiveError", "You are no longer a member of this chat.");
            return;
        }

        var currentUserModel = await userService.GetCurrentContextUserAsync();
        var messageModel = new MessageModel
        {
            Id = Guid.NewGuid().ToString(),
            ChatId = chatId,
            Content = message,
            SenderId = currentUserModel.Id,
            TimeCreated = DateTime.Now,
            Status = MessageStatus.Sent
        };

        await messageService.CreateAsync(messageModel);

        // Convert profile picture to base64 if it exists
        string profilePicture = null;
        if (currentUserModel.Picture != null && currentUserModel.Picture.Length > 0)
        {
            profilePicture = Convert.ToBase64String(currentUserModel.Picture);
        }

        await Clients.Group(chatId).SendAsync("ReceiveMessage", 
            user, 
            message, 
            chatMembersUserNames, 
            messageModel.TimeCreated.ToString("o"),
            messageModel.Id,
            profilePicture);

        var otherMembers = chat.ChatUsers
            .Where(cu => cu.UserId != currentUserModel.Id)
            .ToList();

        foreach (var member in otherMembers)
        {
            try 
            {
                logger.LogInformation($"Creating notification for user {member.UserId} from {currentUserModel.UserName}");
                
                bool shouldSendNotification = chat.Type == ChatType.Group;

                if (!shouldSendNotification)
                {
                    var areFriends = await friendService.GetAllFriendsAsync(member.UserId);
                    shouldSendNotification = areFriends.Any(f => f.Id == currentUserModel.Id);
                }

                if (shouldSendNotification)
                {
                    var notification = new NotificationModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = member.UserId,
                        SenderUsername = currentUserModel.UserName,
                        Message = chat.Type == ChatType.Group ? 
                            $"[{chat.Name}] {message}" : message,
                        TimeCreated = DateTime.Now,
                        IsRead = false,
                        Type = NotificationType.Message,
                        RelatedEntityId = chatId
                    };

                    await notificationService.CreateAsync(notification);
                    await notificationHubContext.Clients.User(member.UserId)
                        .SendAsync("NotificationsUpdated", notification);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"❌ Error processing notification for user {member.UserId}");
            }
        }
    }

    public async Task EditMessage(string messageId, string newContent, string chatId)
    {
        try
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var message = await messageService.GetByIdAsync(messageId);

            if (message.SenderId != currentUser.Id)
            {
                await Clients.Caller.SendAsync("ReceiveError", "You can only edit your own messages.");
                return;
            }

            message.Content = newContent;
            message.LastUpdated = DateTime.UtcNow;
            message.IsEdited = true;

            await messageService.UpdateAsync(message);

            await Clients.Group(chatId).SendAsync("MessageEdited", 
                messageId, 
                newContent, 
                message.LastUpdated.ToString("o"),
                message.IsEdited);
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveError", "Failed to edit message.");
            logger.LogError(ex, "Error editing message");
        }
    }

    public async Task MarkMessageAsDelivered(string messageId)
    {
        var message = await messageService.GetByIdAsync(messageId);
        if (message != null && message.Status == MessageStatus.Sent)
        {
            message.Status = MessageStatus.Delivered;
            await messageService.UpdateAsync(message);
            await Clients.Group(message.ChatId).SendAsync("MessageStatusUpdated", messageId, "Delivered");
        }
    }

    public async Task MarkMessageAsSeen(string messageId)
    {
        try 
        {
            var message = await messageService.GetByIdAsync(messageId);
            if (message != null && message.Status != MessageStatus.Read)
            {
                message.Status = MessageStatus.Read;
                await messageService.UpdateAsync(message);
                await Clients.Group(message.ChatId).SendAsync("MessageStatusUpdated", messageId, "Read");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error marking message as seen: {MessageId}", messageId);
            // Don't rethrow - we don't want to break the client connection for this
        }
    }

    public async Task DeleteMessage(string messageId)
    {
        var currentUser = await userService.GetCurrentContextUserAsync();
        var message = await messageService.GetByIdAsync(messageId);

        if (message == null)
        {
            logger.LogWarning("Message not found: {MessageId}", messageId);
            return;
        }

        if (message.SenderId != currentUser.Id)
        {
            logger.LogWarning("User {UserId} attempted to delete a message they did not send: {MessageId}", currentUser.Id, messageId);
            return;
        }

        await messageService.DeleteByIdAsync(messageId);
        await Clients.Group(message.ChatId).SendAsync("MessageDeleted", messageId);
    }
}
