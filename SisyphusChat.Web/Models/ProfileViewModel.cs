using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SisyphusChat.Core.Models;

namespace SisyphusChat.Web.Models
{
    public class ProfileViewModel
    {
        public string Username { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string UserId { get; set; }
    }
}