using SisyphusChat.Core.Models;

namespace SisyphusChat.Web.Models;

public class ChatPageViewModel
{
    public string ChatId { get; set; }

    public ChatViewModel ChatViewModel { get; set; }

    public UserViewModel UserViewModel { get; set; }
}