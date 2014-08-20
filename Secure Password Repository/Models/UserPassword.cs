using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Models
{

    [Table("UserPassword")]
    public class UserPassword
    {

        public Int32 Id { get; set; }
        public Int32 PasswordId { get; set; }

    }

}