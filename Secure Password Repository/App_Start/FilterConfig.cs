using System.Web;
using System.Web.Mvc;
using Secure_Password_Repository.Filters;

namespace Secure_Password_Repository
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new UserPasswordCookieVerificationAndEncryption());     //register a custom filter
        }
    }
}
