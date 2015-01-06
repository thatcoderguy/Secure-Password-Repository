using Secure_Password_Repository.Identity;
using Secure_Password_Repository.Services;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secure_Password_Repository.Models
{

    [Table("UserPassword")]
    public class UserPassword
    {
        
        [Key]
        [Column(Order=1)]
        public Int32 Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public Int32 PasswordId { get; set; }

        public virtual Password UsersPassword { get; set; }

        public virtual ApplicationUser UserPasswordUser { get; set; }

        public bool CanEditPassword { get; set; }

        public bool CanDeletePassword { get; set; }

        public bool CanViewPassword { get; set; }

        public bool CanChangePermissions { get; set; }

    }

}