using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Models
{

    [Table("Password")]
    public class Password
    {
        [Key]
        public Int32 PasswordId { get; set; }
        public string Description { get; set; }
        public string EncryptedUserName { get; set; }
        public string EncryptedSecondCredential { get; set; }
        public string EncryptedPassword { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public Int32 CategoryId { get; set; }
        public bool Deleted { get; set; }

    }

}