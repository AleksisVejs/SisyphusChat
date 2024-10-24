using System.ComponentModel.DataAnnotations;
using SisyphusChat.Core.Models;

namespace SisyphusChat.Web.Models
{
    public class CreateGroupChatViewModel
    {
        [Required(ErrorMessage = "Group name is required.")]
        [StringLength(25, MinimumLength = 1, ErrorMessage = "Group name should be between 1 and 25 characters.")]
        public string ChatName { get; set; }

        [Required(ErrorMessage = "At least one user must be selected.")]
        [MinLength(1, ErrorMessage = "At least one user must be selected.")]
        public List<string> SelectedUserNames { get; set; } = new List<string>();

        public List<UserModel> Users { get; set; } = new List<UserModel>();
    }
}