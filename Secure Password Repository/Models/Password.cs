using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Secure_Password_Repository.Models
{

    [Table("Password")]
    public class Password
    {
        [Key]
        public Int32 PasswordId { get; set; }

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

        public Int32 Parent_CategoryId { get; set; }

        [ScriptIgnore]
        public virtual Category Parent { get; set; }

        public bool Deleted { get; set; }

        public Int16 PasswordOrder { get; set; }

    }

}