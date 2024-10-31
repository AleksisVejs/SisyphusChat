using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;

namespace SisyphusChat.Web.Hubs;

public class ChatHub(
    IMessageService messageService,
    IUserService userService,
    IChatService chatService
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
        Id = Guid.NewGuid().ToString(),
        ChatId = chatId,
        Content = message,
        SenderId = currentUserModel.Id,
        TimeCreated = DateTime.Now,
        LastUpdated = DateTime.Now,
    };

    await messageService.CreateAsync(messageModel);

    await Clients.Group(chatId).SendAsync("ReceiveMessage", 
        user, 
        message, 
        chatMembersUserNames, 
        messageModel.TimeCreated.ToString("o"),
        messageModel.Id); // Include the real message ID
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