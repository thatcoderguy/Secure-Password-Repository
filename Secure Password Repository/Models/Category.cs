using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Models
{
    public class Category
    {

        [Key]
        public int CatagoryId { get; set; }
        public string CategoryName { get; set; }
        public Int16 ParentCategoryId { get; set; }

    }
}