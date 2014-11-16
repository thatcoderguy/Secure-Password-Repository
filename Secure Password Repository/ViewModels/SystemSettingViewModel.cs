using Secure_Password_Repository.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

        [EmailAddress(ErrorMessage = "This field must either be empty or contain an email address")]
        [Display(Name = "Username for the above mailbox")]
        public string SMTPServerUsername { get; set; }

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
        [Display(Name = "Minimum user level allowed to edit categories")]
        public ApplicationRole RoleAllowEditCategories { get; set; }

        [Required]
        [Display(Name = "Minimum user level allowed to delete categories")]
        public ApplicationRole RoleAllowDeleteCategories { get; set; }

        [Required]
        [Display(Name = "Minimum user level allowed to add categories")]
        public ApplicationRole RoleAllowAddCategories { get; set; }

        [Required]
        [Display(Name = "Minimum user level allowed to add passwords")]
        public ApplicationRole RoleAllowAddPasswords { get; set; }

        [Required]
        [Display(Name = "Administrators have access to all passwords")]
        public bool AdminsHaveAccessToAllPasswords { get; set; }

        [Required]
        [Display(Name = "Automatically broadcast a category position update to all clients")]
        public bool BroadcastCategoryPositionChange { get; set; }

        [Required]
        [Display(Name = "Automatically broadcast a password position update to all clients")]
        public bool BroadcastPasswordPositionChange { get; set; }

        public IEnumerable<ApplicationRole> AvailableRoles { get; set; }
    }
}