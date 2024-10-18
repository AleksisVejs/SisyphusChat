using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Services;
using SisyphusChat.Web.Models;

namespace SisyphusChat.Web.Controllers
{
    public class FriendsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> AddFriends()
        {
            return View();
        }

        // public async Task<IActionResult> SendRequest(string receiverid){}

        // public async Task<IActionResult> CancelRequest(string receiverid){}

        // public async Task<IActionResult> AcceptRequest(){}

        // public async Task<IActionResult> DenyRequest(){}
        
        // public async Task<IActionResult> RemoveFriend(friendid){}
    }
}
