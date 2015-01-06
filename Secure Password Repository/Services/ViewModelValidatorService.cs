using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.WebPages.Html;


namespace Secure_Password_Repository.Services
{
    class ViewModelValidatorService: IViewModelValidatorService
    {
        private ModelStateDictionary ModelState;

        public bool IsPostedModelValid()
        {
            throw new NotImplementedException();
        }

        public void AddError(string key, string errorMessage)
        {
            ModelState.AddError(key, errorMessage);
        }
    }
}
