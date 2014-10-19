using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
        [Display(Name = "Email address to send emails from")]
        public string SMTPEmailAddress { get; set; }

        [Required]
        [Display(Name = "Username for the above mailbox")]
        public string SMTPServerUsername { get; set; }

        [Required]
        [Display(Name = "Password for the above mailbox")]
        public string SMTPServerPassword { get; set; }

        [Required]
        [Display(Name = "Use this to tweak SCrypt cost to your environment")]
        public string SCryptHashCost { get; set; }

        [Required]
        [Display(Name = "Use this to tweak the number of PBKDF2 iterations to your environment")]
        public string PBKDF2IterationCount { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to edit categories")]
        public string RoleAllowEditCategories { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to delete categories")]
        public string RoleAllowDeleteCategories { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to add categories")]
        public string RoleAllowAddCategories { get; set; }

        [Required]
        [Display(Name = "Lowest role allowed to add passwords")]
        public string RoleAllowAddPasswords { get; set; }

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