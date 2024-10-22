using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Services;
using SisyphusChat.Web.Models;
using NuGet.Protocol.Plugins;
using SisyphusChat.Infrastructure.Exceptions;
using SisyphusChat.Infrastructure.Migrations;

namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    public class FriendsController(IFriendService friendService, IUserService userService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var friends = await friendService.GetAllFriendsAsync(currentUser.Id);
            return View(friends);
        }

        public async Task<IActionResult> Add()
        {
            return View();
        }

        public async Task<IActionResult> Requests()
        {
            return View();
        }

        // Creates a friendship request
        [HttpPost]
        public async Task<IActionResult> SendRequest(string receiverUsername)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var receiverUser = await userService.GetByUsernameAsync(receiverUsername);
            await friendService.SendRequestAsync(currentUser.Id, receiverUser.Id);
            return RedirectToAction("AddFriends");
        }

        // Cancels friendship request
        [HttpPost]
        public async Task CancelRequest(string receiverId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            await friendService.DeleteByIdAsync(currentUser.Id + ' ' + receiverId);
        }

        // Accepts friendship request
        [HttpPost]
        public async Task AcceptRequest(string senderId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            var friend = await friendService.GetByIdAsync(senderId + ' ' + currentUser.Id);
            await friendService.UpdateAsync(friend);
        }

        // Denies friendship request
        [HttpPost]
        public async Task DenyRequest(string senderId)
        {
            var currentUser = await userService.GetCurrentContextUserAsync();
            await friendService.DeleteByIdAsync(senderId + ' ' + currentUser.Id);
        }

        // Stops friendship
        [HttpPost]
        public async Task RemoveFriend(string friendId)
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
        }
    }
}
