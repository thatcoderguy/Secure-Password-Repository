using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.ViewModels
{

    public class PasswordBase
    {
        [Display(Name = "Description - Displayed in the list (so make this meaningful e.g. Company PayPal Login)")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Username")]
        [Required]
        public string EncryptedUserName { get; set; }

        [Display(Name = "Optional Secondary Credential")]
        public string EncryptedSecondCredential { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string EncryptedPassword { get; set; }

        [Display(Name = "Location - Ideally a URL")]
        [Required]
        public string Location { get; set; }

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }
    }

    public class PasswordItem
    {
        public Int32 PasswordId { get; set; }
        public string Description { get; set; }
        public Int32 Parent_CategoryId { get; set; }
        //public ApplicationUser currentUserPassword { get; set; }
        ICollection<PasswordUserPermission> PasswordUser { get; set; }
    }

    public class PasswordEdit : PasswordBase
    {
        [Required]
        public Int32 PasswordId { get; set; }
    }

    public class PasswordAdd : PasswordBase
    {
        [Required]
        public Int32 Parent_CategoryId { get; set; }
    }

    public class PasswordUserPermission
    {
        public Int32 Id { get; set; }
        public Int32 PasswordId { get; set; }
        public bool CanEditPassword { get; set; }
        public bool CanDeletePassword { get; set; }
        public bool CanViewPassword { get; set; }
    }
}