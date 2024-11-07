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
    public class AdminController(IAdminService adminService,
                                 INotificationService notificationService,
                                 UserManager<User> userManager,
                                 IHubContext<NotificationHub> notificationHubContext,
                                 IUserService userService,
                                 IUserDeletionService userDeletionService,
                                 ApplicationDbContext context) : Controller
    {
        private readonly IAdminService _adminService = adminService;
        private readonly INotificationService _notificationService = notificationService;
        private readonly UserManager<User> _userManager = userManager;
        private readonly IHubContext<NotificationHub> _notificationHubContext = notificationHubContext;
        private readonly IUserService _userService = userService;
        private readonly IUserDeletionService _userDeletionService = userDeletionService;
        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> DownloadReport(string reportType, string format)
        {
            byte[] reportBytes;

            if (format == "excel")
            {
                reportBytes = await adminService.GenerateExcelAsync(reportType);
                return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportType}_Report.xlsx");
            }
            else
            {
                reportBytes = await adminService.GeneratePdfAsync(reportType);
                return File(reportBytes, "application/pdf", $"{reportType}_Report.pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PreviewReport(string reportType)
        {
            try
            {
                // To generate a pdf report with specified parameter to preview instead of downloading
                byte[] stream = await adminService.GeneratePdfAsync(reportType);
                return File(stream, "application/pdf"); // return the pdf file to preview in browser instead of downloading

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

            var users = await _userManager.Users
                .Where(u => u.Id != user.Id && !u.IsDeleted) // Exclude current user and admins
                .ToListAsync();
            
            var reports = await _context.Reports
                .Include(r => r.Chat)
                .Include(r => r.Message)
                .Include(r => r.ReportedUser)
                .OrderByDescending(r => r.TimeCreated)
                .ToListAsync();

            // Calculate report counts for each user
            var userReportCounts = await _context.Reports
                .GroupBy(r => r.ReportedUserId)
                .Select(g => new { UserId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(
                    x => x.UserId,
                    x => x.Count
                );

            var viewModel = new AdminViewModel
            {
                Users = users,
                Reports = reports,
                UserReportCounts = userReportCounts
            };

            return View(viewModel);
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
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.IsAdmin)
            {
                return Json(new { success = false, message = "Cannot ban administrators" });
            }

            var banStart = DateTime.UtcNow;
            DateTime? banEnd = duration switch
            {
                BanDuration.TwentyFourHours => banStart.AddHours(24),
                BanDuration.OneWeek => banStart.AddDays(7),
                BanDuration.OneMonth => banStart.AddMonths(1),
                BanDuration.Permanent => null,
                _ => throw new ArgumentException("Invalid ban duration")
            };

            var banNotification = new NotificationModel
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                Message = $"Your account has been banned for {duration}.",
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

            return Json(new { 
                success = true, 
                banEnd = banEnd?.ToString("o"),
                message = $"User {user.UserName} has been banned successfully."
            });
        }

        [HttpPost]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            var admin = await _userManager.GetUserAsync(User);
            if (admin == null || !admin.IsAdmin)
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
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

            return Json(new { 
                success = true, 
                message = $"User {user.UserName} has been unbanned successfully."
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReport(string reportId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !user.IsAdmin)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            await _adminService.DeleteReportAsync(reportId);
            return RedirectToAction(nameof(Index));
        }
    }

    public class AdminNotificationRequest
    {
        public string Message { get; set; }
    }
}
