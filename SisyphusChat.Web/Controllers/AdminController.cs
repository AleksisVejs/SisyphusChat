using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Services;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Core.Models;
using SisyphusChat.Web.Hubs;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SisyphusChat.Web.Models;
using SisyphusChat.Attributes;
using SisyphusChat.Infrastructure.Data;

namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    [AdminOnly]
    public class AdminController : Controller
    {
        private readonly IReportService _reportService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IUserService _userService;
        private readonly IUserDeletionService _userDeletionService;
        private readonly ApplicationDbContext _context;
        public AdminController(
            IReportService reportService,
            INotificationService notificationService,
            UserManager<User> userManager,
            IHubContext<NotificationHub> notificationHubContext,
            IUserService userService,
            IUserDeletionService userDeletionService,
            ApplicationDbContext context)
        {
            _reportService = reportService;
            _notificationService = notificationService;
            _userManager = userManager;
            _notificationHubContext = notificationHubContext;
            _userService = userService;
            _userDeletionService = userDeletionService;
            _context = context;
        }

        public async Task<IActionResult> DownloadReport(string reportType, string format)
        {
            byte[] reportBytes;

            if (format == "excel")
            {
                reportBytes = await _reportService.GenerateExcelAsync(reportType);
                return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportType}_Report.xlsx");
            }
            else
            {
                reportBytes = await _reportService.GeneratePdfAsync(reportType);
                return File(reportBytes, "application/pdf", $"{reportType}_Report.pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PreviewReport(string reportType)
        {
            try
            {
                byte[] stream = await _reportService.GeneratePdfAsync(reportType);
                return File(stream, "application/pdf");

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

                return StatusCode(500, $"An error occurred while generating the report: {reportType}.");

            }


        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var users = await _userManager.Users
                .Where(u => u.Id != currentUser.Id) // Exclude current user
                .ToListAsync();

            return View(new AdminViewModel { Users = users });
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] AdminNotificationRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return Forbid();
            }

            try
            {
                if (string.IsNullOrWhiteSpace(request.Message))
                {
                    return BadRequest("Message cannot be empty");
                }

                var users = await _userManager.Users.ToListAsync();
                
                foreach (var targetUser in users)
                {
                    var notificationModel = new NotificationModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        UserId = targetUser.Id,
                        Message = request.Message,
                        Type = NotificationType.AdminMessage,
                        TimeCreated = DateTime.UtcNow,
                        IsRead = false,
                        RelatedEntityId = Guid.NewGuid().ToString(),
                        SenderUsername = "System Admin"
                    };
                    
                    await _notificationService.CreateAsync(notificationModel);
                    await _notificationHubContext.Clients.User(targetUser.Id)
                        .SendAsync("ReceiveNotification", notificationModel);
                }
                
                return Ok(new { message = "Notification sent successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to send notification", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BanUser(string userId, BanDuration duration)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !admin.IsAdmin)
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsAdmin)
            {
                return BadRequest("Cannot ban administrators");
            }

            var banMessage = duration switch
            {
                BanDuration.TwentyFourHours => "24 hours",
                BanDuration.OneWeek => "one week",
                BanDuration.OneMonth => "one month",
                BanDuration.Permanent => "permanently",
                _ => throw new ArgumentException("Invalid ban duration")
            };

            var banStart = DateTime.UtcNow;
            DateTime? banEnd = duration switch
            {
                BanDuration.TwentyFourHours => banStart.AddHours(24),
                BanDuration.OneWeek => banStart.AddDays(7),
                BanDuration.OneMonth => banStart.AddMonths(1),
                BanDuration.Permanent => null, // Null indicates permanent ban
                _ => throw new ArgumentException("Invalid ban duration")
            };

            var banNotification = new NotificationModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Message = $"Your account has been banned for {banMessage}.",
                Type = NotificationType.AdminMessage,
                TimeCreated = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = userId,
                SenderUsername = "System"
            };

            await _notificationService.CreateAsync(banNotification);
            await _notificationHubContext.Clients.User(userId).SendAsync("ReceiveNotification", banNotification);

            user.IsBanned = true;
            user.BanStart = banStart;
            user.BanEnd = banEnd;
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = $"User {user.UserName} has been banned for {banMessage}.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !admin.IsAdmin)
            {
                return Forbid();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var unbanNotification = new NotificationModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Message = "Your account has been unbanned by an administrator. You can now access the system again.",
                Type = NotificationType.AdminMessage,
                TimeCreated = DateTime.UtcNow,
                IsRead = false,
                RelatedEntityId = userId,
                SenderUsername = "System"
            };

            await _notificationService.CreateAsync(unbanNotification);
            await _notificationHubContext.Clients.User(userId).SendAsync("ReceiveNotification", unbanNotification);

            user.IsBanned = false;
            user.BanStart = null;
            user.BanEnd = null;
            await _userManager.UpdateAsync(user);

            TempData["SuccessMessage"] = $"User {user.UserName} has been unbanned.";
            return RedirectToAction(nameof(Index));
        }
    }

    public class AdminNotificationRequest
    {
        public string Message { get; set; }
    }
}
