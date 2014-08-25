using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.ViewModels
{
    public class CategoryChildrenViewModel
    {
        public ICollection<Category> CategoryItems { get; set; }
        public ICollection<Password> PasswordItems { get; set; }
    }
}