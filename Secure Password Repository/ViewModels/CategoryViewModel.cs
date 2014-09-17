using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.ViewModels
{
    public class CategoryViewModel
    {
        [Required]
        public string CategoryName { get; set; }  
        [Required]
        public Int32? Category_ParentID { get; set; } 
        public virtual ICollection<CategoryModel> SubCategories { get; set; } 
        public virtual ICollection<PasswordModel> Passwords { get; set; }
        public Int16 CategoryOrder { get; set; }
    }
}