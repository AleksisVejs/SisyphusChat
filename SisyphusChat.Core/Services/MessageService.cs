using AutoMapper;
using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using SisyphusChat.Infrastructure.Entities;

namespace SisyphusChat.Core.Services
{
    public class MessageService(IUnitOfWork unitOfWork, IMapper mapper) : IMessageService
    {
        public async Task CreateAsync(MessageModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var messageEntity = mapper.Map<Message>(model);

            await unitOfWork.MessageRepository.AddAsync(messageEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(MessageModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var messageEntity = mapper.Map<Message>(model);

            await unitOfWork.MessageRepository.UpdateAsync(messageEntity);
            await unitOfWork.SaveAsync();
        }

        public async Task<ICollection<MessageModel>> GetAllAsync()
        {
            var messageEntities = await unitOfWork.MessageRepository.GetAllAsync();

            return mapper.Map<ICollection<Message>, ICollection<MessageModel>>(messageEntities);
        }

        public async Task<MessageModel> GetByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            var messageEntity = await unitOfWork.MessageRepository.GetByIdAsync(id);

            return mapper.Map<MessageModel>(messageEntity);
        }

        public async Task DeleteByIdAsync(string id)
        {
            ArgumentException.ThrowIfNullOrEmpty(id);

            await unitOfWork.MessageRepository.DeleteByIdAsync(id);
            await unitOfWork.SaveAsync();
        }
    }
}