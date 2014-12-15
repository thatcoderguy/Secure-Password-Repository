using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Secure_Password_Repository.Repositories
{
    public class CategoryRepository : ICategoryRepository 
    {
        private ApplicationDbContext DatabaseContext;
        private IPermissionService PermissionService;

        public CategoryRepository(ApplicationDbContext context, IPermissionService permissionservice)
        {
            this.DatabaseContext = context;
            this.PermissionService = permissionservice;
        }

        public Category GetCategoryWithChildren(int categoryid)
        {
            Category ReturnCategoryItem = DatabaseContext.Categories
                                            .Where(c => c.CategoryId == categoryid)
                                            .Include(c => c.SubCategories)
                                            .Include(c => c.Passwords)
                                            .Include(c => c.Passwords.Select(p => p.Creator))
                                            .ToList()
                                            .Select(c => new Category()
                                            {

                                                SubCategories = c.SubCategories
                                                                    //make sure only undeleted subcategories are returned
                                                                    .Where(sub => !sub.Deleted)
                                                                    .OrderBy(sub => sub.CategoryOrder)
                                                                    .ToList(),

                                                Passwords = c.Passwords
                                                                    //make sure only undeleted passwords - that the current user has acccess to - are returned
                                                                    .Where(pass => !pass.Deleted && PermissionService.CanViewPassword(pass)) 
                                                                    .OrderBy(pass => pass.PasswordOrder)
                                                                    .ToList(),

                                                CategoryId = c.CategoryId,
                                                CategoryName = c.CategoryName,
                                                Category_ParentID = c.Category_ParentID,
                                                CategoryOrder = c.CategoryOrder,
                                                Parent_Category = c.Parent_Category,
                                                Deleted = c.Deleted
                                            })
                                            .SingleOrDefault();

            return ReturnCategoryItem;
        }

        public void InsertCategory(Category category)
        {
            throw new NotImplementedException();
        }

        public void DeleteCategory(int categoryid)
        {
            throw new NotImplementedException();
        }

        public void UpdateCategory(Category category)
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