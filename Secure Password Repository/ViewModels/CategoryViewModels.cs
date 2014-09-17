using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.ViewModels
{
    public class CategoryAdd
    {
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public Int32? Category_ParentID { get; set; }
    }

    public class CategoryEdit
    {
        public Int32? CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }

    public class CategoryList
    {
        public Int32 CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
        [Required]
        public Int32? Category_ParentID { get; set; }
        public virtual ICollection<CategoryList> SubCategories { get; set; }
        public virtual ICollection<PasswordList> Passwords { get; set; }
    }

    public class CategoryDisplayTree
    {
        public CategoryList categoryListItem { get; set; }
        public CategoryAdd categoryAddItem { get; set; }
        public PasswordAdd passwordAddItem { get; set; }
    }

    public class CategoryDisplayItem
    {
        public CategoryList categoryListItem { get; set; }
        public CategoryEdit categoryEditItem { get; set; }
    }

}