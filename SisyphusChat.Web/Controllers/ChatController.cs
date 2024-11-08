using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Models;
using SisyphusChat.Web.Models;
using SisyphusChat.Infrastructure.Exceptions;
using SisyphusChat.Core.Services;


namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    public class ChatController(IAttachmentService attachmentService, IChatService chatService, IUserService userService,
                                IFriendService friendService, IReportService reportService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var users = await friendService.GetAllFriendsAsync(currentUser.Id);
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
            var users = await friendService.GetAllFriendsAsync(currentUser.Id);
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

        public async Task<IActionResult> OpenGroupChat(string chatId)
        {
            var chat = await chatService.GetByIdAsync(chatId);

            return RedirectToAction("ChatRoom", new { chatId = chat.Id });
        }

        public async Task<IActionResult> ReportMessage(string chatId, string messageId, ReportType type, string reason)
        {
            var chat = await chatService.GetByIdAsync(chatId);
            var message = chat.Messages.FirstOrDefault(m => m.Id == messageId);

            if (chat == null || message == null)
            {
                throw new EntityNotFoundException("Chat or Message not found");
            }

            var report = new ReportModel
            {
                ChatId = chat.Id,
                MessageId = message.Id,
                ReportedUserId = message.SenderId,
                Type = type,
                Reason = reason
            };

            await reportService.CreateAsync(report);

            return RedirectToAction("ChatRoom", new { chatId = chat.Id });
        }

        public async Task<IActionResult> ReportChat(string chatId, ReportType type, string reason)
        {
            var chat = await chatService.GetByIdAsync(chatId);

            if (chat == null)
            {
                throw new EntityNotFoundException("Chat not found");
            }

            var report = new ReportModel
            {
                ChatId = chat.Id,
                ReportedUserId = chat.OwnerId,
                Type = type,
                Reason = reason
            };

            await reportService.CreateAsync(report);

            return RedirectToAction("ChatRoom", new { chatId = chat.Id });
        }

        [HttpPost]
        public async Task<IActionResult> UploadAttachment(IFormFile file, string messageId)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Convert file to byte array
            byte[] fileContent;
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                fileContent = ms.ToArray();
            }

            var attachment = new AttachmentModel
            {
                MessageId = messageId,
                FileName = file.FileName,
                Content = fileContent
            };

            await attachmentService.CreateAsync(attachment);

            return Ok(new { success = true });
        }

        public async Task<IActionResult> DownloadAttachment(string attachmentId)
        {
            var attachment = await attachmentService.GetByIdAsync(attachmentId);
            if (attachment == null) return NotFound();

            return File(attachment.Content, "application/octet-stream", attachment.FileName);
        }
    }
}
