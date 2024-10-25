using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SisyphusChat.Web.Controllers;

public class GroupChatController(IUserService userService, IChatService chatService, IFriendService friendService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var currentUser = await userService.GetCurrentContextUserAsync();
        var users = await friendService.GetAllFriendsAsync(currentUser.Id);

        var createGroupChatViewModel = new CreateGroupChatViewModel
        {
            Users = users.ToList(),
        };

        return View(createGroupChatViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroupChat(CreateGroupChatViewModel model)
    {
        if (ModelState.IsValid)
        {
            var chatModel = new ChatModel
            {
                Name = model.ChatName,
                Type = ChatType.Group,
                OwnerId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Set the current user's ID as OwnerId
            };

            // Fetch users with resolved UserIds
            var chatUsers = await userService.GetUsersByUserNamesAsync(model.SelectedUserNames.ToArray());

            // Add these users to the chat
            chatModel.ChatUsers = chatUsers.ToList();

            await chatService.CreateAsync(chatModel);
            return RedirectToAction("Index", "Chat");
        }

        return View(model);
    }

}