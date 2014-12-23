using Microsoft.AspNet.Identity.EntityFramework;
using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Secure_Password_Repository.Repositories
{
    public class CategoryRepository : ICategoryRepository 
    {
        private ApplicationDbContext databasecontext;
        private int userid;

        public CategoryRepository(ApplicationDbContext context, int userid)
        {
            this.databasecontext = context;
            this.userid = userid;
        }

        public Category GetCategoryWithChildren(int categoryid, List<int> accessiblepasswordids, bool usercanoverridepasswords)
        {
            Category ReturnCategoryItem = databasecontext.Categories
                                            .Where(c => c.CategoryId == categoryid)
                                            .Include(c => c.SubCategories)
                                            .Include(c => c.Passwords)
                                            .Include(c => c.Passwords.Select(p => p.Creator))
                                            .ToList()
                                            .Select(c => new Category()
                                            {

                                                SubCategories = c.SubCategories
                                                                    .Where(sub => !sub.Deleted)
                                                                    .OrderBy(sub => sub.CategoryOrder)
                                                                    .ToList(),                          //make sure only undeleted subcategories are returned

                                                Passwords = c.Passwords
                                                                    .Where(pass => !pass.Deleted
                                                                                                && (accessiblepasswordids.Contains(pass.PasswordId)
                                                                                                    || usercanoverridepasswords
                                                                                                    || pass.Creator_Id == userid)
                                                                           )   //make sure only undeleted passwords - that the current user has acccess to - are returned
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