using System.ComponentModel.DataAnnotations;
using SisyphusChat.Core.Models;

namespace SisyphusChat.Web.Models;

public class ChatSettingsViewModel
{
    public string CurrentUser { get; set; }

    public string ChatId { get; set; }

    public string ChatOwner { get; set; }

    public string ChatName { get; set; }

    [StringLength(25, ErrorMessage = "Group name max length is 25 characters.")]
    public string NewChatName { get; set; }

    public string SelectedUsers { get; set; }

    public ICollection<UserModel> ChatUsers { get; set; }

    public ICollection<UserModel> NotChatUsers { get; set; }
}