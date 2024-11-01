using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using Microsoft.Extensions.Logging;
using SisyphusChat.Infrastructure.Entities;

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
            LastUpdated = DateTime.Now,
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
            profilePicture,
            messageModel.Id);

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
                        Message = chat.Type == ChatType.Group ? $"[{chat.Name}] {message}" : message,
                        TimeCreated = DateTime.UtcNow,
                        IsRead = false,
                        Type = NotificationType.Message,
                        RelatedEntityId = chatId
                    };

                    logger.LogInformation($"Attempting to save notification: {notification.Id} to database");
                    await notificationService.CreateAsync(notification);
                    logger.LogInformation($"✅ Successfully saved notification {notification.Id} to database");

                    logger.LogInformation($"Attempting to send notification to user {member.UserId}");
                    await notificationHubContext.Clients.User(member.UserId)
                        .SendAsync("ReceiveNotification", notification);
                    logger.LogInformation($"✅ Successfully sent notification to user {member.UserId}");
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

            await messageService.UpdateAsync(message);

            await Clients.Group(chatId).SendAsync("MessageEdited", 
                messageId, 
                newContent, 
                message.LastUpdated.ToString("o"));
        }
        catch (Exception ex)
        {
            await Clients.Caller.SendAsync("ReceiveError", "Failed to edit message.");
            throw;
        }
    }
}
