using Microsoft.AspNetCore.Mvc;
using SisyphusChat.Core.Interfaces;
using System.Threading.Tasks;

public class ReportController(IReportService reportService) : Controller
{

    [HttpGet]
    public async Task<IActionResult> DownloadReport(string reportType)
    {
        var stream = await reportService.GenerateExcelAsync(reportType);
        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{reportType}.xlsx");
    }
    public IActionResult Index()
    {
        return View("~/Views/Admin/Index.cshtml");
    }

}
