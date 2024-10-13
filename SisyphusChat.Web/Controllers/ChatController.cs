using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public ChatController(IChatService chatService, IUserService userService, UserManager<User> userManager)
        {
            _chatService = chatService;
            _userService = userService;
            _userManager = userManager;
        }

        // Fetches list of all users for the dropdown
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // Creates or opens a private 1-on-1 chat
        [HttpPost]
        public async Task<IActionResult> CreateOrOpenChat(string recipientUserId)
        {
            var currentUserId = _userManager.GetUserId(User);
            var chat = await _chatService.OpenOrCreatePrivateChatAsync(currentUserId, recipientUserId);
            return RedirectToAction("ChatRoom", new { chatId = chat.Id });
        }

        public IActionResult ChatRoom(Guid chatId)
        {
            ViewBag.ChatId = chatId;
            return View();
        }
    }
}
