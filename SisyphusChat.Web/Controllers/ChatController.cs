using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Services;
using SisyphusChat.Web.Models;

namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    public class ChatController(IChatService chatService, IUserService userService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var users = await userService.GetAllExceptCurrentUserAsync(currentUser);
            var associatedChats = await chatService.GetAssociatedChatsAsync(currentUser);

            var userViewModel = new UserViewModel
            {
                Users = users.ToList(),
                CurrentUser = currentUser,
                AssociatedChats = associatedChats.ToList()
            };

            return View(userViewModel);
        }


        // Creates or opens a private 1-on-1 chat
        [HttpPost]
        public async Task<IActionResult> CreateOrOpenChat(string recipientUserId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var chat = await chatService.OpenOrCreatePrivateChatAsync(currentUser.Id, recipientUserId);
            return RedirectToAction("ChatRoom", new { chatId = chat.Id });
        }

        public async Task<IActionResult> ChatRoom(Guid chatId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var chat = await chatService.GetByIdAsync(chatId.ToString());
            var users = await userService.GetAllExceptCurrentUserAsync(currentUser);
            var chatViewModel = new ChatViewModel { Chat = chat };

            if (chat == null)
            {
                return NotFound();
            }

            ViewBag.ChatName = chat.Name;
            ViewBag.ChatId = chatId;

            return View(chatViewModel);
        }
    }
}
