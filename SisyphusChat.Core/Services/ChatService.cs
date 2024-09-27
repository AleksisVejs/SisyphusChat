/*using SisyphusChat.Core.Interfaces;
using SisyphusChat.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SisyphusChat.Infrastructure.Entities;
using SisyphusChat.Infrastructure.Exceptions;
using SisyphusChat.Infrastructure.Interfaces;

namespace SisyphusChat.Core.Services
{
    public class ChatService(IUnitOfWork unitOfWork) : IChatService
    {
        public async Task CreateAsync(ChatModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            var chatEntity = <Chat>(model);

            await unitOfWork.ChatRepository.AddAsync(chatEntity);
            await unitOfWork.SaveAsync();
        }

    }
}
*/