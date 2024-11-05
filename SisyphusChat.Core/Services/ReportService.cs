using AutoMapper;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Services
{
    public class ReportService(IUnitOfWork unitOfWork, IMapper mapper) : IReportService
    {
        public async Task CreateAsync(ReportModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var reportEntity = mapper.Map<Report>(model);

            await unitOfWork.ReportRepository.AddAsync(reportEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ReportModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var reportEntity = mapper.Map<Report>(model);

            await unitOfWork.ReportRepository.UpdateAsync(reportEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task<ICollection<ReportModel>> GetAllAsync()
        {
            var reportEntities = await unitOfWork.ReportRepository.GetAllAsync();

            return mapper.Map<ICollection<Report>, ICollection<ReportModel>>(reportEntities);
        }

        public async Task<ReportModel> GetByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            var reportEntity = await unitOfWork.ReportRepository.GetByIdAsync(id);

            return mapper.Map<ReportModel>(reportEntity);
        }

        public async Task DeleteByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            await unitOfWork.ReportRepository.DeleteByIdAsync(id);
            await unitOfWork.SaveAsync();
        }
    }
}