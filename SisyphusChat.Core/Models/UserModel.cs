using System.Reflection.Metadata;

namespace SisyphusChat.Core.Models
{
    public class UserModel
    {
        public string ID { get; set; }

        public string UserName { get; set; }

        public Blob Picture { get; set; }

        public bool IsOnline { get; set; }

        
    }
}