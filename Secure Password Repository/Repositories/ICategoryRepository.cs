using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Repositories
{
    public interface ICategoryRepository : IDisposable
    {
        Category GetCategoryWithChildren(int categoryid, List<int> accessiblepasswordids, bool usercanoverridepermissions);
        void InsertCategory(Category category);
        void DeleteCategory(int categoryid);
        void UpdateCategory(Category category);
        void Save();
    }
}
