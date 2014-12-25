using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Secure_Password_Repository.Repositories
{
    public interface IPasswordRepository : IDisposable
    {
        List<Password> GetPasswordsByParentId(int parentid);
        List<int> GetPasswordIdsByParentId(int parentid);
        Password GetPasswordById(int passwordid);
        int GetPasswordCreatorId(int passwordid);
        void InsertPassword(Password password);
        void DeletePassword(int passwordid);
        void UpdatePassword(Password password);
        void Save();
    }
}