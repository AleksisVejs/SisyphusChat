﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SisyphusChat.Infrastructure.DTO
{
    public class UserWithLastMessageDto
    {
        // Custom data transfer object for users with last message for specific data

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string LastMessageContent { get; set; }
        public string LastMessageSenderId { get; set; }
        public DateTime? LastMessageDate { get; set; }
    }
}
