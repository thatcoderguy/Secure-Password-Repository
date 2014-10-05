using Secure_Password_Repository.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
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
            var bundleList = bundlecontext.BundleCollection.Where(b => b.Path.StartsWith(bundlename)).ToList();
            
            //store the path of each bundle item in the returned list
            foreach (Bundle bundleItem in bundleList)
            {
                bundlePaths.Add(bundleItem.Path);
            }

            //render the scripts contrained in the bundle list
            return Scripts.Render(bundlePaths.ToArray());
        }

        /// <summary>
        /// Generates the HtmlString contents for a selected script bundle that needs to be rendered on the view
        /// </summary>
        /// <param name="BundleName">
        /// Name of the bundle to render in the view
        /// </param>
        /// <returns>
        /// HtmlString formatted data of the script bundles to be rendered
        /// </returns>
        public static IHtmlString RenderScripts(string BundleName)
        {

            //a string list to store all of the bundle paths to be rendered
            List<string> bundlePaths = new List<string>();

            string bundlename = "~/bundles/" + BundleName;

            HttpContext currentHttpContext = HttpContext.Current;
            var httpContext = new HttpContextWrapper(currentHttpContext);

            BundleContext bundlecontext = new BundleContext(httpContext, BundleTable.Bundles, "~/bundles/");
            bundlecontext.EnableInstrumentation = false;

            //return a list of bundle items that start with the name of the current controller
            var bundleList = bundlecontext.BundleCollection.Where(b => b.Path.StartsWith(bundlename)).ToList();

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
        /// <param name="controllerName">Name of the controller that the view belongs to</param>
        /// <param name="viewName">Name of the view to render</param>
        /// <param name="model">Name of the view model</param>
        /// <returns>String containing partial view HTML</returns>
        class FakeController : ControllerBase { protected override void ExecuteCore() { } }
        public static string RenderPartialToString(string controllerName, string viewName, object model)
        {
            using (var writer = new StringWriter())
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", controllerName);
                var fakeControllerContext = new ControllerContext(new HttpContextWrapper(new HttpContext(HttpContext.Current.Request, new HttpResponse(null))), routeData, new FakeController());
                var razorViewEngine = new RazorViewEngine();
                var razorViewResult = razorViewEngine.FindPartialView(fakeControllerContext, viewName, false);

                var viewContext = new ViewContext(fakeControllerContext, razorViewResult.View, new ViewDataDictionary(model), new TempDataDictionary(), writer);
                razorViewResult.View.Render(viewContext, writer);
                return writer.ToString();
            }
        }

    }


}