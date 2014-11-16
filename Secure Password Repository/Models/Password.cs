using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Secure_Password_Repository.Models
{

    [Table("Password")]
    public class Password
    {
        [Key]
        public Int32 PasswordId { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string EncryptedUserName { get; set; }
        
        public string EncryptedSecondCredential { get; set; }
        
        [Required]
        public string EncryptedPassword { get; set; }

        [Required]
        public string Location { get; set; }
        
        public string Notes { get; set; }
        
        public Int32 Parent_CategoryId { get; set; }
        
        public virtual Category Parent_Category { get; set; }
        
        public bool Deleted { get; set; }
        
        public Int16 PasswordOrder { get; set; }
        
        public virtual ApplicationUser Creator { get; set; }
        
        public Int32 Creator_Id { get; set; }
        
        public virtual ICollection<UserPassword> Parent_UserPasswords { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }

}