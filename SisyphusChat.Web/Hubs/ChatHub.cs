using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Core.Services;

namespace SisyphusChat.Web.Hubs;

public class ChatHub(
    IMessageService messageService,
    IUserService userService,
    IChatService chatService,
    INotificationService notificationService
    ) : Hub
{
    public override async Task OnConnectedAsync()
    {
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

        await Clients.Group(chatId).SendAsync("ReceiveMessage", user, message, messageModel.TimeCreated.ToString("o"));

        // Iterate through the chat members to send notifications
        foreach (var memberUsername in chatMembersUserNames)
        {
            Console.WriteLine($"Processing notification for: {memberUsername}");

            // Skip sending notification to the sender
            if (memberUsername != user)
            {
                // Retrieve user ID based on the member's username
                var memberUser = await userService.GetByUsernameAsync(memberUsername); // Adjusted to retrieve by username

                if (memberUser == null)
                {
                    Console.WriteLine($"Error: Could not retrieve user {memberUsername}");
                }
                else
                {
                    var notification = new NotificationModel
                    {
                        Id = Guid.NewGuid().ToString(), // Generate a unique ID for the notification
                        Message = $"New message from {user}",
                        Timestamp = DateTime.UtcNow,
                        SenderUsername = user, // The username of the sender
                        UserId = memberUser.Id, // The ID of the member receiving the notification
                        IsRead = false,
                        NotificationType = "UnseenMessage"
                    };

                    // Now we can safely call AddNotificationAsync with the correct parameters
                    await notificationService.AddNotificationAsync(memberUser.Id, "UnseenMessage", user); // user is the sender's username

                    // Optionally, send a real-time notification to the user
                    await Clients.User(memberUser.Id).SendAsync("ReceiveNotification", notification);

                }
            }
        }
    }


    public async Task GetNotifications()
    {
        var currentUser = await userService.GetCurrentContextUserAsync();
        var notifications = await notificationService.GetUserNotificationsAsync(currentUser.Id);

        // Send notifications to the connected client
        await Clients.Caller.SendAsync("LoadNotifications", notifications);
    }

}