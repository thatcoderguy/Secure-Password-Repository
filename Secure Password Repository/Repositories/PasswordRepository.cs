using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Secure_Password_Repository.Services;
using Secure_Password_Repository.Exceptions;

namespace Secure_Password_Repository.Repositories
{
    public class PasswordRepository: IPasswordRepository 
    {
        private ApplicationDbContext context;
        private IUserPasswordRepository userPasswordRepository;
        private IPermissionService permissionService;
        private IAccountService accountService;

        public PasswordRepository(ApplicationDbContext databasecontext, IPermissionService permissionservice, IUserPasswordRepository userpasswordrepository, IAccountService accountservice)
        {
            this.context = databasecontext;
            this.userPasswordRepository = userpasswordrepository;
            this.permissionService = permissionservice;
            this.accountService = accountservice;
        }

        public List<Password> GetPasswordsByParentId(int categoryid)
        {
            return context.Passwords
                            .Where(pass => pass.Parent_CategoryId == categoryid)
                            .Include(p => p.Parent_UserPasswords)
                            .Where(pass => pass.Parent_UserPasswords.Any(up => up.Id == accountService.GetUserId()))
                            .ToList();
        }

        public List<int> GetPasswordIdsByParentId(int categoryid)
        {
            return context.Passwords
                            .Where(pass => pass.Parent_CategoryId == categoryid)
                            .Include(p => p.Parent_UserPasswords)
                            .Where(pass => pass.Parent_UserPasswords.Any(up => up.Id == accountService.GetUserId()))
                            .Select(p => p.PasswordId)
                            .ToList();
        }

        public int GetPasswordCreatorId(int passwordid)
        {
            var PasswordItem = context.Passwords.SingleOrDefault(p => p.PasswordId == passwordid);

            if(PasswordItem == null)
                throw new PasswordItemNotFoundException("Password Item Could Not Be Found");

            return PasswordItem.Creator_Id;
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