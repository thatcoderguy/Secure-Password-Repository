using Microsoft.AspNet.Identity;
using Secure_Password_Repository.Extensions;
using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.ViewModels
{

    public class PasswordItem
    {
        public Int32 PasswordId { get; set; }
        public string Description { get; set; }
        public Int32 Parent_CategoryId { get; set; }
        public ApplicationUser Creator { get; set; }
        public IList<PasswordUserPermission> Parent_UserPasswords { get; set; }
        public bool CanDeletePassword
        {
            get
            {
                return (Parent_UserPasswords != null && Parent_UserPasswords.Any(up => up.Id == HttpContext.Current.User.Identity.GetUserId().ToInt() && up.CanDeletePassword)) || (Creator != null && Creator.Id == HttpContext.Current.User.Identity.GetUserId().ToInt());
            }
        }
    }

    public class PasswordDisplay
    {
        [Required]
        public Int32 PasswordId { get; set; }
        public ApplicationUser Creator { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Username")]
        [Required]
        public string EncryptedUserName { get; set; }

        [Display(Name = "Secondary Credential")]
        public string EncryptedSecondCredential { get; set; }

        [Display(Name = "Location")]
        [Required]
        public string Location { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string EncryptedPassword { get; set; }

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }

        public bool isLink()
        {
            return !string.IsNullOrEmpty(Location) && Location.Length > 7 && (Location.Substring(0, 7) == "http://" || Location.Substring(0, 8) == "https://");
        }
    }

    public class PasswordEdit
    {
        [Required]
        public Int32 PasswordId { get; set; }

        [Display(Name = "Description - Displayed in the list (so make this meaningful e.g. Company PayPal Login)")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Username")]
        [Required]
        public string EncryptedUserName { get; set; }

        [Display(Name = "Optional Secondary Credential")]
        public string EncryptedSecondCredential { get; set; }

        [Display(Name = "Location (prepend http:// or https:// to create a link)")]
        [Required]
        public string Location { get; set; }

        [Display(Name = "Password")]
        public string EncryptedPassword { get; set; }

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }
    }

    public class PasswordAdd
    {
        [Required]
        public Int32 Parent_CategoryId { get; set; }

        [Display(Name = "Description - Displayed in the list (so make this meaningful e.g. Company PayPal Login)")]
        [Required]
        public string Description { get; set; }

        [Display(Name = "Username")]
        [Required]
        public string EncryptedUserName { get; set; }

        [Display(Name = "Optional Secondary Credential")]
        public string EncryptedSecondCredential { get; set; }

        [Display(Name = "Location (prepend http:// or https:// to create a link)")]
        [Required]
        public string Location { get; set; }

        [Display(Name = "Password")]
        [Required]
        public string EncryptedPassword { get; set; }

        [Display(Name = "Additional Notes")]
        public string Notes { get; set; }
    }

    public class PasswordDelete
    {
        public Int32 PasswordId { get; set; }
    }

    public class PasswordPassword
    {
        public string PlainTextPassword { get; set; }
    }

    public class PasswordUserPermission
    {
        [Required]
        public Int32 Id { get; set; }

        [Required]
        public Int32 PasswordId { get; set; }

        [Required]
        public bool CanEditPassword { get; set; }

        [Required]
        public bool CanDeletePassword { get; set; }

        [Required]
        public bool CanViewPassword { get; set; }

        [Required]
        public bool CanChangePermissions { get; set; }

        public ApplicationUser UserPasswordUser { get; set; }
    }

    public class PasswordDetails
    {
        public PasswordDisplay ViewPassword { get; set; } 
        
        public PasswordEdit EditPassword { get; set; }

        public IList<PasswordUserPermission> UserPermissions { get; set; }

        public DefaultTab OpenTab;

        public bool CanEditPassword 
        { 
            get 
            {
                if (UserPermissions == null || ViewPassword == null)
                    return false;
                else
                    return (UserPermissions != null && UserPermissions.Any(up => up.Id == HttpContext.Current.User.Identity.GetUserId().ToInt() && up.CanEditPassword)) || (ViewPassword.Creator != null && ViewPassword.Creator.Id == HttpContext.Current.User.Identity.GetUserId().ToInt());
            } 
        }

        public bool CanChangePermissions
        {
            get
            {
                if (UserPermissions == null || ViewPassword == null)
                    return false;
                else
                    return (UserPermissions != null && UserPermissions.Any(up => up.Id == HttpContext.Current.User.Identity.GetUserId().ToInt() && up.CanChangePermissions)) || (ViewPassword.Creator != null && ViewPassword.Creator.Id == HttpContext.Current.User.Identity.GetUserId().ToInt());
            }
        }
    }

    public enum DefaultTab { ViewPassword, EditPassword, EditPermissions };
}