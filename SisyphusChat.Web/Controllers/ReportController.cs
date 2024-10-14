using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Services;
using System.Threading.Tasks;

public class ReportController(IReportService reportService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> DownloadReport(string reportType)
    {
        try
        {
            byte[] pdfBytes = await reportService.GeneratePdfAsync(reportType);
            return File(pdfBytes, "application/pdf", $"{reportType}Report.pdf");
        }
        catch (Exception ex)
        {
            // Log the exception
            return StatusCode(500, "An error occurred while generating the report.");
        }

    }
    [HttpGet]
    public async Task<IActionResult> PreviewReport(string reportType)
    {
        try
        {
            byte[] stream = await reportService.GeneratePdfAsync(reportType);
            return File(stream, "application/pdf");

        }
        catch(Exception ex)
        {
                   
            return StatusCode(500, "An error occurred while generating the report.");

        }
        

    }


    public IActionResult Index()
    {
        return View("~/Views/Admin/Index.cshtml");
    }

}
