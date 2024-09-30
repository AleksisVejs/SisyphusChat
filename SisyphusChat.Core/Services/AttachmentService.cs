using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Services
{
    public class AttachmentService(IUnitOfWork unitOfWork, IMapper mapper) : IAttachmentService
    {
        public async Task CreateAsync(Attachment model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var attachmentEntity = mapper.Map<Attachment>(model);

            await unitOfWork.AttachmentRepository.AddAsync(attachmentEntity);
            await unitOfWork.SaveAsync();
        }
        public async Task<Attachment> GetByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            var attachmentEntity = await unitOfWork.AttachmentRepository.GetByIdAsync(id);
            return attachmentEntity;
        }

        public async Task UpdateAsync(Attachment model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var attachmentEntity = mapper.Map<Attachment>(model);

            await unitOfWork.AttachmentRepository.UpdateAsync(attachmentEntity);
            await unitOfWork.SaveAsync();
        }


        public async Task<ICollection<Attachment>> GetAllByMessageIdAsync(string messageId)
        {
            ArgumentException.ThrowIfNullOrEmpty(messageId);

            var attachmentEntities = await unitOfWork.AttachmentRepository.GetAllByMessageIdAsync(messageId);

            return mapper.Map<ICollection<Attachment>, ICollection<Attachment>>(attachmentEntities);
        }
        public async Task<ICollection<Attachment>> GetAllAsync()
        {
            var attachmentEntities = await unitOfWork.AttachmentRepository.GetAllAsync();

            return mapper.Map<ICollection<Attachment>, ICollection<Attachment>>(attachmentEntities);
        }

        public async Task DeleteByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            await unitOfWork.AttachmentRepository.DeleteByIdAsync(id);
            await unitOfWork.SaveAsync();
        }
    }
}