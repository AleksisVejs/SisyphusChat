using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace SisyphusChat.Web.Controllers;

public class GroupChatController(IUserService userService, IChatService chatService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var currentUser = await userService.GetCurrentContextUserAsync();
        var users = await userService.GetAllExceptCurrentUserAsync(currentUser);

        var createGroupChatViewModel = new CreateGroupChatViewModel
        {
            OwnerId = currentUser.Id,
            Users = users.ToList(),
        };

        return View(createGroupChatViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroupChat(CreateGroupChatViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.Users = (await userService.GetAllExceptCurrentUserAsync(await userService.GetCurrentContextUserAsync())).ToList();
            return View("Index", model);
        }

        var chatUsers = await userService.GetUsersByUserNamesAsync(model.SelectedUserNames.ToArray());

        chatUsers.Add(new ChatUserModel { UserId = model.OwnerId });

        var currentUser = await userService.GetCurrentContextUserAsync();

        var chatModel = new ChatModel
        {
            OwnerId = model.OwnerId,
            Owner = currentUser,
            Name = model.ChatName,
            Type = ChatType.Group,
            ChatUsers = chatUsers,
            TimeCreated = DateTime.UtcNow,
            LastUpdated = DateTime.UtcNow
        };

        await chatService.CreateAsync(chatModel);

        return RedirectToAction("Index", "Chat");
    }
}