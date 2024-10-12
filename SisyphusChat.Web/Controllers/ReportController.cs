using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using System.Threading.Tasks;

public class ReportController(IReportService reportService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> DownloadReport(string reportType)
    {
        var stream = await reportService.GeneratePdfAsync(reportType);
        // Return the PDF file with the correct MIME type and .pdf extension
        return File(stream, "application/pdf", $"{reportType}.pdf");
    }
    [HttpGet]
    public async Task<IActionResult> PreviewReport(string reportType)
    {
        var stream = await reportService.GeneratePdfAsync(reportType);
        return File(stream, "application/pdf");
    }


    public IActionResult Index()
    {
        return View("~/Views/Admin/Index.cshtml");
    }

}
