using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.UI;
using System.Web.WebPages;

namespace Secure_Password_Repository.Extensions
{
    /// <summary>
    /// Utility class to render views and bundles in ways they couldn't normally be rendered
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

        /// <summary>
        /// Populates a Partial View with data from a supplied model and then returns the view as a string
        /// </summary>
        /// <param name="partialViewName">Name of the partial view</param>
        /// <param name="viewModel">Name of the view model</param>
        /// <returns>String containing partial view HTML</returns>
        public static string RenderPartialToString(string partialViewName, object viewModel)
        {
            ViewPage viewPage = new ViewPage() { ViewContext = new ViewContext() };

            viewPage.ViewData = new ViewDataDictionary(viewModel);
            viewPage.Controls.Add(viewPage.LoadControl(partialViewName));

            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                using (HtmlTextWriter tw = new HtmlTextWriter(sw))
                {
                    viewPage.RenderControl(tw);
                }
            }

            return sb.ToString();
        }


    }


}