using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

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

        public Password UsersPassword { get; set; }
        public ApplicationUser UserPasswordUser { get; set; }

    }

}