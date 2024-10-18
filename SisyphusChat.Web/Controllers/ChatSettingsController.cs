using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Web.Models;

namespace SisyphusChat.Web.Controllers;

[Authorize]
public class ChatSettingsController(IUserService userService, IChatService chatService) : Controller
{
    public async Task<IActionResult> Index(string chatId)
    {
        var currentUser = await userService.GetCurrentContextUserAsync();
        var users = await userService.GetAllAsync();
        var chat = await chatService.GetByIdAsync(chatId);
        var chatOwner = chat.Owner.UserName;
        var chatUsers = chat.ChatUsers.Select(it => it.User).Where(c => c.Id != chat.Owner.Id);
        var usersNotInChat = users
            .Where(user => chat.ChatUsers
                .Select(m => m.User)
                .All(chatUser => chatUser.Id != user.Id && user.Id != chat.Owner.Id)).ToList();

        var viewModel = new ChatSettingsViewModel
        {
            CurrentUser = currentUser.UserName,
            ChatName = chat.Name,
            ChatOwner = chatOwner,
            ChatUsers = chatUsers.ToList(),
            NotChatUsers = usersNotInChat
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateGroupChat(ChatSettingsViewModel model)
    {

        List<string> selectedUsers;

        if (!model.SelectedUsers.IsNullOrEmpty())
        {
            selectedUsers = JsonConvert.DeserializeObject<List<string>>(model.SelectedUsers);
        }
        else
        {
            selectedUsers = new List<string>();
        }


        if (!string.IsNullOrWhiteSpace(model.NewChatName))
        {
            model.ChatName = model.NewChatName;
        }

        await chatService.UpdateWithMembersAsync(
            new ChatModel
            {
                Name = model.ChatName,
                Id = model.ChatId
            }, selectedUsers);

        return RedirectToAction("Chat", "Chat", new { chatId = model.ChatId });
    }

    public async Task<IActionResult> DeleteUserFromChat(string userId, string chatId)
    {
        try
        {
            await chatService.DeleteUserFromChatByIdAsync(userId, chatId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}