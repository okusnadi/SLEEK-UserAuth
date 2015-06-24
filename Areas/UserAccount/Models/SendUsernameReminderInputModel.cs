using System.ComponentModel.DataAnnotations;

namespace WebHost.Areas.UserAccount.Models
{
    public class SendUsernameReminderInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}