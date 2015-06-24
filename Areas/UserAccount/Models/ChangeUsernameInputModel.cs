using System.ComponentModel.DataAnnotations;

namespace WebHost.Areas.UserAccount.Models
{
    public class ChangeUsernameInputModel
    {
        [Required]
        public string NewUsername { get; set; }
    }
}