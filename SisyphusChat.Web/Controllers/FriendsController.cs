using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Services;
using SisyphusChat.Web.Models;
using NuGet.Protocol.Plugins;
using SisyphusChat.Infrastructure.Exceptions;
using SisyphusChat.Core.Models;
using Microsoft.AspNetCore.SignalR;
// ... existing using statements ...
namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    public class FriendsController(IFriendService friendService, IUserService userService,IHubContext<NotificationHub> notificationHubContext,INotificationService notificationService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var model = new FRequestModel
            {
                Friends = await friendService.GetAllFriendsAsync(currentUser.Id),
                SentRequests = await friendService.GetAllSentRequestsAsync(currentUser.Id),
                ReceivedRequests = await friendService.GetAllReceivedRequestsAsync(currentUser.Id)
            };
            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        public async Task<IActionResult> Requests()
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            FRequestModel requests = new FRequestModel();
            requests.SentRequests = await friendService.GetAllSentRequestsAsync(currentUser.Id);
            requests.ReceivedRequests = await friendService.GetAllReceivedRequestsAsync(currentUser.Id);
            return View(requests);
        }
        [HttpPost]
        public async Task<IActionResult> SendRequest(string receiverUsername)
        {
            ViewData["Title"] = "Add Friends Page";

            var currentUser = await userService.GetCurrentContextUserAsync();
            var receiverUser = await userService.GetByUsernameAsync(receiverUsername);
            

            if (receiverUser == null)
            {
                ModelState.AddModelError("receiverUsername", "User not found.");
                return View("Add"); // Return to the Add view with ModelState errors
            }

            try
            {
                await friendService.SendRequestAsync(currentUser.Id, receiverUser.Id);
                
                // Create and send notification
                var notification = new NotificationModel
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = receiverUser.Id,
                    Message = $"{currentUser.UserName} sent you a friend request",
                    Type = NotificationType.FriendRequest,
                    TimeCreated = DateTime.UtcNow,
                    IsRead = false,
                    RelatedEntityId = Guid.NewGuid().ToString(),
                    SenderUsername = currentUser.UserName
                };
                await notificationService.CreateAsync(notification);
                
                // Send real-time notification
                await notificationHubContext.Clients.User(receiverUser.Id)
                    .SendAsync("ReceiveNotification", notification);

                TempData["SuccessMessage"] = "Friend request sent successfully!";
                return RedirectToAction("Add");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("receiverUsername", ex.Message);
                return View("Add"); // Return to the Add view with ModelState errors
            }
        }

        // Cancels friendship request
        [HttpPost]
        public async Task<IActionResult> CancelRequest(string receiverId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            await friendService.DeleteByIdAsync(currentUser.Id + ' ' + receiverId);
            return RedirectToAction("Index");
        }

        // Accepts friendship request
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(string senderId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var friend = await friendService.GetByIdAsync(senderId + ' ' + currentUser.Id);
            await friendService.UpdateAsync(friend);
            return RedirectToAction("Index");
        }

        // Denies friendship request
        [HttpPost]
        public async Task<IActionResult> DenyRequest(string senderId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            await friendService.DeleteByIdAsync(senderId + ' ' + currentUser.Id);
            return RedirectToAction("Index");
        }

        // Stops friendship
        [HttpPost]
        public async Task<IActionResult> RemoveFriend(string friendId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            
            try
            {
                await friendService.DeleteByIdAsync(currentUser.Id + ' ' + friendId);
            }
            catch (EntityNotFoundException)
            {
                await friendService.DeleteByIdAsync(friendId + ' ' + currentUser.Id);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Profile(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            // Fetch user data based on the username
            var user = await userService.GetByUsernameAsync(username);
            if (user == null)
            {
                return NotFound();
            }

            var profile = new ProfileViewModel
            {
                Username = user.UserName,
                ProfilePicture = user.Picture,
                UserId = user.Id,
                IsOnline = user.IsOnline,
                JoinDate = user.TimeCreated
            };

            return View(profile);
        }
    }
}
