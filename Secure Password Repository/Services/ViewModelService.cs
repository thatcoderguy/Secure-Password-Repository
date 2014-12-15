using Secure_Password_Repository.Database;
using Secure_Password_Repository.Models;
using Secure_Password_Repository.Repositories;
using Secure_Password_Repository.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web;

namespace Secure_Password_Repository.Services
{
    [assembly: WebActivatorEx.PreApplicationStartMethod(typeof(ViewModelService), "AutoMapperStart")]
    public class ViewModelService : IViewModelService
    {
        private ICategoryRepository CategoryRepository;
        private IPasswordRepository PasswordRepository;
        private 
        

        public ViewModelService()
        {
            this.CategoryRepository = new CategoryRepository(new ApplicationDbContext(), new PermissionService(IAccountService new AccountService(HttpCon, new UserPasswordRepository(new ApplicationDbContext()), new ApplicationSettingsService()));
            this.PasswordRepository = new PasswordRepository(new ApplicationDbContext(), new PermissionService(Thread.CurrentPrincipal, new UserPasswordRepository(new ApplicationDbContext()), new ApplicationSettingsService()));
            
            //Thread.CurrentPrincipal
        }

        public ViewModelService(HttpContextBase httpcontextbase, IPrincipal securityprincipal)
        {
            this.CategoryRepository = new CategoryRepository(new ApplicationDbContext(), new PermissionService(IAccountService new AccountService(httpcontextbase, ), new UserPasswordRepository(new ApplicationDbContext()), new ApplicationSettingsService()));
            this.PasswordRepository = new PasswordRepository(new ApplicationDbContext(), new PermissionService(Thread.CurrentPrincipal, new UserPasswordRepository(new ApplicationDbContext()), new ApplicationSettingsService()));
            
        }

        public ViewModelService(ICategoryRepository categoryrepository, IPasswordRepository passwordrepository, IModelValidatorService modelvalidatorservice, HttpContextBase httpcontextbase, IPrincipal securityprincipal)
        {
            this.CategoryRepository = categoryrepository;
            this.PasswordRepository = passwordrepository;
        }

        public ViewModelService(ICategoryRepository categoryrepository, IPasswordRepository passwordrepository, IModelValidatorService modelvalidatorservice)
        {
        }

        /// <summary>
        /// Setup all of the automapper maps that will be used throughout this controller - this is called by WebActivatorEx
        /// </summary>
        public static void AutoMapperStart()
        {
            AutoMapper.Mapper.CreateMap<Category, CategoryItem>();
            AutoMapper.Mapper.CreateMap<Category, CategoryDelete>();
            AutoMapper.Mapper.CreateMap<CategoryAdd, Category>();
            AutoMapper.Mapper.CreateMap<CategoryEdit, Category>();

            AutoMapper.Mapper.CreateMap<Category[], IList<CategoryItem>>();
            AutoMapper.Mapper.CreateMap<Password[], IList<PasswordItem>>();
            AutoMapper.Mapper.CreateMap<UserPassword[], IList<PasswordUserPermission>>().ReverseMap();

            AutoMapper.Mapper.CreateMap<UserPassword, PasswordUserPermission>().ReverseMap();

            AutoMapper.Mapper.CreateMap<Password, PasswordItem>();
            AutoMapper.Mapper.CreateMap<Password, PasswordEdit>();
            AutoMapper.Mapper.CreateMap<Password, PasswordDisplay>();
            AutoMapper.Mapper.CreateMap<Password, PasswordDelete>();
            AutoMapper.Mapper.CreateMap<PasswordAdd, Password>();
            AutoMapper.Mapper.CreateMap<PasswordEdit, PasswordDisplay>();
        }

        public CategoryDisplayItem GetCategoryDisplayItem(int parentcategoryid)
        {
            //get the root node, and include it's subcategories
            Category rootCategoryItem = CategoryRepository.GetCategoryWithChildren(parentcategoryid);

            if(rootCategoryItem==null)




        }
    }
}