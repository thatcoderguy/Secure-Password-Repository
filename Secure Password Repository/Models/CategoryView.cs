using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace Secure_Password_Repository.Models
{
    public class CategoryView
    {
        public CategoryView()
        {
            SubCategories = new List<Category>();
            Passwords = new List<Password>();
            Parent_Category = new Category();
        }

        public Int32 CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Int32? Category_ParentID { get; set; }
        [ScriptIgnore]
        public virtual Category Parent_Category { get; set; }
        [ScriptIgnore]
        public virtual List<Category> SubCategories { get; set; }
        [ScriptIgnore]
        public virtual List<Password> Passwords { get; set; }
        public Int16 CategoryOrder { get; set; }
        public bool Deleted { get; set; }
    }
}