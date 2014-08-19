using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Models
{

    public class Category
    {

        [Key]
        public Int32 CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Int32 ParentCategoryId { get; set; }
        public Int16 CategoryOrder { get; set; }
        public bool Deleted { get; set; }
        public bool SubCategory { get; set; }
        

    }
}