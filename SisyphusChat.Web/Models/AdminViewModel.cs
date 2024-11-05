using SisyphusChat.Infrastructure.Entities;

public class AdminViewModel
{
    public IEnumerable<User> Users { get; set; }
    public IEnumerable<Report> Reports { get; set; }
    public Dictionary<string, int> UserReportCounts { get; set; }
} 