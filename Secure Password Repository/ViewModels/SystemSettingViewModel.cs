using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Secure_Password_Repository.ViewModels
{
    public class SystemSettingViewModel
    {
        [Required]
        [Display(Name="Path to logo image")]
        public string LogoImage { get; set; }

        [Required]
        [Display(Name="IP Address or Domain for the SMTP Server")]
        public string SMTPServerAddress { get; set; }

        [Required]
        [EmailAddress(ErrorMessage="This field must contain an email address")]
        [Display(Name = "Email address to send emails from")]
        public string SMTPEmailAddress { get; set; }

        [Required(AllowEmptyStrings=true)]
        [EmailAddress(ErrorMessage = "This field must either be empty or contain an email address")]
        [Display(Name = "Username for the above mailbox")]
        public string SMTPServerUsername { get; set; }

        [Required(AllowEmptyStrings = true)]
        [Display(Name = "Password for the above mailbox")]
        public string SMTPServerPassword { get; set; }

        [Required]
        [RegularExpression("([0-9]+)", ErrorMessage="This field must contain a number")]
        [Display(Name = "Use this to tweak SCrypt cost to your environment")]
        public string SCryptHashCost { get; set; }

        [Required]
        [RegularExpression("([0-9]+)", ErrorMessage = "This field must contain a number")]
        [Display(Name = "Use this to tweak the number of PBKDF2 iterations to your environment")]
        public string PBKDF2IterationCount { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to edit categories")]
        public SelectList RoleAllowEditCategories { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to delete categories")]
        public SelectList RoleAllowDeleteCategories { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to add categories")]
        public SelectList RoleAllowAddCategories { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to add passwords")]
        public SelectList RoleAllowAddPasswords { get; set; }

        [Required]
        [Display(Name = "Administrators have access to all passwords")]
        public bool AdminsHaveAccessToAllPasswords { get; set; }

        [Required]
        [Display(Name = "Automatically update all logged in clients when a category's position is updated")]
        public bool BroadcastCategoryPositionChange { get; set; }

        [Required]
        [Display(Name = "Automatically update all logged in clients when a password's position is updated")]
        public bool BroadcastPasswordPositionChange { get; set; }
    }
}