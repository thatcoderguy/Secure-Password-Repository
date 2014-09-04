using System.Web;
using System.Web.Optimization;

namespace Secure_Password_Repository
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery-ui").Include(
                        "~/Scripts/jquery-ui.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/fancybox").Include(
                      "~/Scripts/jquery.fancybox.pack.js",
                      "~/Scripts/jquery.fancybox-buttons.js",
                      "~/Scripts/jquery.fancybox-media.js",
                      "~/Scripts/jquery.fancybox-thumbs.js",
                      "~/Scripts/jquery.mousewheel-*"));

            bundles.Add(new ScriptBundle("~/bundles/password_treeviewandforms").Include(
                       "~/Scripts/treeview.js",
                       "~/Scripts/categoryandpassword.js",
                       "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/utilities").Include(
                        "~/Scripts/utilities.js"));
                       

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/treeview.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/jquery.fancybox.css",
                      "~/Content/jquery.fancybox-thumbs.css",
                      "~/Content/jquery.fancybox-buttons.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
