using BrockAllen.MembershipReboot;
using System.ComponentModel.DataAnnotations;

namespace WebHost.Areas.UserAccount.Models
{
    public class PasswordResetSecretsViewModel
    {
        public PasswordResetSecret[] Secrets { get; set; }
    }

    public class AddSecretQuestionInputModel
    {
        [Required]
        public string Question { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Answer { get; set; }
    }
}