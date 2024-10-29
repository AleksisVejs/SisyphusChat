using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Services;
using System.Threading.Tasks;

namespace SisyphusChat.Web.Controllers
{
    [Authorize]
    public class AdminController(IAdminService reportService) : Controller
    {
        public async Task<IActionResult> DownloadReport(string reportType, string format)
        {
            byte[] reportBytes;

            if (format == "excel")
            {
                reportBytes = await reportService.GenerateExcelAsync(reportType);
                return File(reportBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportType}_Report.xlsx");
            }
            else
            {
                reportBytes = await reportService.GeneratePdfAsync(reportType);
                return File(reportBytes, "application/pdf", $"{reportType}_Report.pdf");
            }
        }

        [HttpGet]
        public async Task<IActionResult> PreviewReport(string reportType)
        {
            try
            {
                // To generate a pdf report with specified parameter to preview instead of downloading
                byte[] stream = await reportService.GeneratePdfAsync(reportType);
                return File(stream, "application/pdf"); // return the pdf file to preview in browser instead of downloading

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());

                return StatusCode(500, $"An error occurred while generating the report: {reportType}."); // Fixed string interpolation

            }


        }


        public IActionResult Index()
        {
            return View("~/Views/Admin/Index.cshtml");
        }

    }

}
