using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Secure_Password_Repository
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        //make sure the session ID persists between user requests
        protected void Session_Start()
        {
            Session["PersistSessionId"] = true;
        }

    }
}
