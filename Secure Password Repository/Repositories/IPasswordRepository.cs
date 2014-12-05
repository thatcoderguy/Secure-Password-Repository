using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Repositories
{
    public interface IPasswordRepository : IDisposable
    {
        List<Password> GetPasswordsByCategoryId(int categoryid);
        List<int> GetPasswordIdsByCategoryId(int categoryid);
        Password GetPasswordById(int passwordid);
        void InsertPassword(Password password);
        void DeletePassword(int passwordid);
        void UpdatePassword(Password password);
        void Save();
    }
}