using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.WebPages;

namespace Secure_Password_Repository.Extensions
{
    /// <summary>
    /// Utility class to render controller-specific scripts and css content
    /// The bundles returned must begin with [controllername]_  e.g.  home_
    /// </summary>
    public static class RenderViewContent
    {
        /// <summary>
        /// Generates the HtmlString contents for script bundles that need to be rendered on the view
        /// </summary>
        /// <returns>
        /// HtmlString formatted data of the script bundles to be rendered
        /// </returns>
        public static IHtmlString RenderScripts()
        {

            //a string list to store all of the bundle paths to be rendered
            List<string> bundlePaths = new List<string>();

            string bundlename = "~/bundles/" + HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower() + "_";

            HttpContext currentHttpContext = HttpContext.Current;
            var httpContext = new HttpContextWrapper(currentHttpContext);

            BundleContext bundlecontext = new BundleContext(httpContext, BundleTable.Bundles, "~/bundles/");
            bundlecontext.EnableInstrumentation = false;
            
            //return a list of bundle items that start with the name of the current controller
            var bundleList = bundlecontext.BundleCollection.ToList().Where(b => b.Path.StartsWith(bundlename));
            
            //store the path of each bundle item in the returned list
            foreach (Bundle bundleItem in bundleList)
            {
                bundlePaths.Add(bundleItem.Path);
            }

            //render the scripts contrained in the bundle list
            return Scripts.Render(bundlePaths.ToArray());
        }

    }
}