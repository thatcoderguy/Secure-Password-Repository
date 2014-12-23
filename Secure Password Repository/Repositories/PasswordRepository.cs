using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Secure_Password_Repository.Repositories
{
    public class PasswordRepository: IPasswordRepository 
    {
        private ApplicationDbContext context;
        private int userid;

        public PasswordRepository(ApplicationDbContext databasecontext, int userid)
        {
            this.context = databasecontext;
            this.userid = userid;
        }

        public List<Password> GetPasswordsByParentId(int categoryid)
        {
            return context.Passwords
                            .Where(pass => pass.Parent_CategoryId == categoryid)
                            .Include(p => p.Parent_UserPasswords)
                            .Where(pass => pass.Parent_UserPasswords.Any(up => up.Id == userid))
                            .ToList();
        }

        public List<int> GetPasswordIdsByParentId(int categoryid)
        {
            return context.Passwords
                            .Where(pass => pass.Parent_CategoryId == categoryid)
                            .Include(p => p.Parent_UserPasswords)
                            .Where(pass => pass.Parent_UserPasswords.Any(up => up.Id == userid))
                            .Select(p => p.PasswordId)
                            .ToList();
        }

        public Password GetPasswordById(int passwordid)
        {
            throw new NotImplementedException();
        }

        public void InsertPassword(Password password)
        {
            throw new NotImplementedException();
        }

        public void DeletePassword(int passwordid)
        {
            throw new NotImplementedException();
        }

        public void UpdatePassword(Password password)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}