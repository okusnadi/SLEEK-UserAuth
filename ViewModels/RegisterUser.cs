using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebHost.ViewModels
{
    public class RegisterUser
    {
        [Display(Name = "Username")]
        [DataType(DataType.EmailAddress)]
        [Required]
        public virtual string UserName { get; set; }
        [DataType(DataType.Password)]
        [MinLength(4)]
        [Required]
        public virtual string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Required]
        public virtual string ConfirmPassword { get; set; }
        [Display(Name = "First Name")]
        [StringLength(50)]
        [Required]
        public virtual string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [StringLength(50)]
        [Required]
        public virtual string LastName { get; set; }        
        [StringLength(50)]
        public virtual string Organisation { get; set; }
        [StringLength(50)]
        public virtual string Title { get; set; }
    }
}