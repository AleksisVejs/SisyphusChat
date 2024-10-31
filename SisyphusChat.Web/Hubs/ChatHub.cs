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
    ILogger<ChatHub> logger
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
            ChatId = chatId,
            Content = message,
            SenderId = currentUserModel.Id,
            TimeCreated = DateTime.UtcNow,
        };

        await messageService.CreateAsync(messageModel);

        await Clients.Group(chatId).SendAsync("ReceiveMessage", user, message, chatMembersUserNames, messageModel.TimeCreated.ToString("o"));

        var otherMembers = chat.ChatUsers
            .Where(cu => cu.UserId != currentUserModel.Id)
            .ToList();

        foreach (var member in otherMembers)
        {
            try 
            {
                logger.LogInformation($"Creating notification for user {member.UserId} from {currentUserModel.UserName}");
                
                var notification = new NotificationModel
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = member.UserId,
                    SenderUsername = currentUserModel.UserName,
                    Message = message,
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
            catch (Exception ex)
            {
                logger.LogError(ex, $"❌ Error processing notification for user {member.UserId}");
            }
        }
    }

}