using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Services;
using System.Threading.Tasks;

public class AdminController(IReportService reportService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> DownloadReport(string reportType)
    {
        // to generate a pdf report and catch errors if any
        try
        {
            // to generate a pdf report with specified parameter to download instantly
            byte[] pdfBytes = await reportService.GeneratePdfAsync(reportType);
            return File(pdfBytes, "application/pdf", $"{reportType}Report.pdf"); // return the pdf file to download from browser
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
            // To generate a pdf report with specified parameter to preview instead of downloading
            byte[] stream = await reportService.GeneratePdfAsync(reportType);
            return File(stream, "application/pdf"); // return the pdf file to preview in browser instead of downloading

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
