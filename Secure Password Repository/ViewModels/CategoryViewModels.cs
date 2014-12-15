using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Secure_Password_Repository.ViewModels
{

    public class CategoryEdit
    {
        public Int32? CategoryId { get; set; }
        [Required]
        public string CategoryName { get; set; }
    }

    public class CategoryAdd : CategoryEdit
    {
        public Int32? Category_ParentID { get; set; }
    }

    public class CategoryDelete
    {
        public Int32? CategoryId { get; set; }
    }

    public class CategoryItem : CategoryEdit
    {
        [Required]
        public Int32 Category_ParentID { get; set; }
        public virtual ICollection<CategoryItem> SubCategories { get; set; }
        public virtual ICollection<PasswordItem> Passwords { get; set; }
        public bool CanEditCategory { get; set; }
        public bool CanDeleteCategory { get; set; }
    }

    public class CategoryDisplayItem
    {
        public CategoryItem categoryListItem { get; set; }
        public CategoryAdd categoryAddItem { get; set; }
        public PasswordAdd passwordAddItem { get; set; }
        public bool CanEditPassword { get; set; }
    }

}