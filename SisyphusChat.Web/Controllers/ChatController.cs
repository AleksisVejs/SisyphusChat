using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Services;
using SisyphusChat.Web.Models;
using SisyphusChat.Infrastructure.Exceptions;
using Microsoft.AspNetCore.SignalR;
using SisyphusChat.Web.Hubs;

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
        public async Task<IActionResult> CreateOrOpenChat(string recipientUserId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            if (recipientUserId == null)
            {
                throw new EntityNotFoundException($"User with AAAAAAA {recipientUserId} not found");
            }
            var chat = await chatService.OpenOrCreatePrivateChatAsync(currentUser.Id, recipientUserId);

            return RedirectToAction("ChatRoom", new { chatId = chat.Id });
        }

        public async Task<IActionResult> ChatRoom(string chatId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var chat = await chatService.GetByIdAsync(chatId);
            var associatedChats = await chatService.GetAssociatedChatsAsync(currentUser);
            var users = await userService.GetAllExceptCurrentUserAsync(currentUser);
            var chatViewModel = new ChatViewModel { Chat = chat };
            var currentChatUserNames = chat.ChatUsers.Select(m => m.User.UserName).ToList();

            var userViewModel = new UserViewModel
            {
                CurrentUser = currentUser,
                Users = users.ToList(),
                AssociatedChats = associatedChats.ToList()
            };

            var viewModel = new ChatPageViewModel
            {
                ChatId = chatId,
                ChatViewModel = chatViewModel,
                UserViewModel = userViewModel,
            };

            if (!currentChatUserNames.Contains(currentUser.UserName))
            {
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateOnlineStatus(bool isOnline)
        {
            var user = await userService.GetCurrentContextUserAsync();
            if (user != null)
            {
                user.LastLogin = DateTime.Now;
                user.IsOnline = isOnline;
                var updateSucceeded = userService.UpdateAsync(user);
                if (updateSucceeded != null)
                {
                    // Notify all clients about the user's status change
                    var hubContext = HttpContext.RequestServices.GetRequiredService<IHubContext<ChatHub>>();
                    await hubContext.Clients.All.SendAsync("UserStatusChanged", user.Id, isOnline);
                    return Ok();
                }
            }
            return BadRequest();
        }


    }
}
