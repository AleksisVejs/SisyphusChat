using SisyphusChat.Core.Models;

namespace SisyphusChat.Web.Models;

public class UserViewModel
{
    public ICollection<UserModel> Users { get; set; }

    public UserModel CurrentUser { get; set; }

    public List<ChatModel> AssociatedChats { get; set; }
}