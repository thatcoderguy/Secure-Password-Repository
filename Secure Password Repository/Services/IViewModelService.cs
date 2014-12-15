using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Services
{
    public interface IViewModelService
    {
        CategoryDisplayItem GetCategoryDisplayItem(int parentcategoryid);
    }
}
