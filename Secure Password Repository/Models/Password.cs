using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Models
{
    public class Password
    {
        [Key]
        public int PasswordId { get; set; }
        public string Description { get; set; }
        public string EncryptedUserName { get; set; }
        public string EncryptedSecondCredential { get; set; }
        public string EncryptedPassword { get; set; }
        public string Location { get; set; }
        public string Notes { get; set; }
        public int CategoryId { get; set; }

    }
}