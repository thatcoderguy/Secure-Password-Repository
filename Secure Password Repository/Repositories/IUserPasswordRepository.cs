using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Secure_Password_Repository.Repositories
{
    public interface IUserPasswordRepository
    {
        List<UserPassword> GetUserPasswordsByCategoryId(int categoryid);
        List<UserPassword> GetUserPasswordsByPasswordId(int passwordid);
        public UserPassword GetUserPassword(int passwordid, int userid);
        void InsertUserPassword(UserPassword userpassword);
        void UpdateUserPassword(UserPassword userpassword);
        void DeleteUserPassword(int userid, int passwordid);
        void Save();
    }
}
