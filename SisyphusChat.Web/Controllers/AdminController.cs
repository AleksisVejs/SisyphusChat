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

namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly IReportService _reportService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public AdminController(
            IReportService reportService,
            INotificationService notificationService,
            UserManager<User> userManager,
            IHubContext<NotificationHub> notificationHubContext)
        {
            _reportService = reportService;
            _notificationService = notificationService;
            _userManager = userManager;
            _notificationHubContext = notificationHubContext;
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
                return Forbid();
            }

            return View("~/Views/Admin/Index.cshtml");
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
    }

    public class AdminNotificationRequest
    {
        public string Message { get; set; }
    }
}
