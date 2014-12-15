using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Secure_Password_Repository.Services;

namespace Secure_Password_Repository.Repositories
{
    public class UserPasswordRepository : IUserPasswordRepository 
    {
        private ApplicationDbContext DatabaseContext;

        public UserPasswordRepository(ApplicationDbContext databasecontext)
        {
            this.DatabaseContext = databasecontext;
        }

        public List<UserPassword> GetUserPasswordsByCategoryId(int categoryid)
        {
            throw new NotImplementedException();
            //var UserPasswordList = databasecontext.UserPasswords.Where(up => up.Id == userid)
                //.Passwords.Where(pass => pass.Parent_CategoryId == parentid).Include(pass => pass.Parent_UserPasswords).Select(pass => pass.Parent_UserPasswords);

            //return UserPasswordList
        }

        public List<UserPassword> GetUserPasswordsByPasswordId(int passwordid)
        {
            throw new NotImplementedException();
        }

        public void InsertUserPassword(UserPassword userpassword)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserPassword(UserPassword userpassword)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserPassword(int userid, int passwordid)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            throw new NotImplementedException();
        }


        public UserPassword GetUserPassword(int passwordid, int userid)
        {
            return DatabaseContext.UserPasswords.FirstOrDefault(up => up.PasswordId == passwordid && up.Id == userid);
        }
    }
}