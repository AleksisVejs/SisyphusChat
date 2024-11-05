using Microsoft.EntityFrameworkCore;
using SisyphusChat.Infrastructure.Data;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Interfaces;
using SisyphusChat.Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;

namespace SisyphusChat.Infrastructure.Repositories;

public class ReportRepository(ApplicationDbContext context) : IReportRepository
{
    public async Task AddAsync(Report entity)
    {
        entity.Id = Guid.NewGuid().ToString();
        entity.TimeCreated = DateTime.Now;

        await context.Reports.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<ICollection<Report>> GetAllAsync()
    {
        return await context.Reports.ToListAsync();
    }

    public async Task<Report> GetByIdAsync(string id)
    {
        var report = await context.Reports.FirstOrDefaultAsync(g => g.Id.ToString() == id);

        if (report == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        return report;
    }

    public async Task DeleteByIdAsync(string id)
    {
        var report = await context.Reports.FindAsync(id);

        if (report == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Reports.Remove(report);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Report entity)
    {
        if (entity == null)
        {
            throw new EntityNotFoundException("Entity not found");
        }

        context.Reports.Update(entity);
        await context.SaveChangesAsync();
    }
}
