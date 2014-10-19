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

            bundles.Add(new ScriptBundle("~/bundles/magnific").Include(
                      "~/Scripts/jquery.magnific-popup.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/passworddetails").Include(
                     "~/Scripts/passworddetails.js",
                     "~/Scripts/jquery.tabbedPanels-1.0.1.min.js",
                     "~/Scripts/ZeroClipboard.js"));

            bundles.Add(new ScriptBundle("~/bundles/password_controller_scripts").Include(
                       "~/Scripts/treeview.js",
                       "~/Scripts/categoryandpassword.js",
                       "~/Scripts/jquery.unobtrusive-ajax.min.js",
                       "~/Scripts/jquery.signalR-{version}.min.js",
                       "~/Scripts/pushnotifications.js"));

            bundles.Add(new ScriptBundle("~/bundles/accountmanager_controller_scripts").Include(
                     "~/Scripts/accountmanager.js"));

            bundles.Add(new ScriptBundle("~/bundles/extensions").Include(
                        "~/Scripts/extensions.js"));
                       
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/treeview.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/magnific-popup.css",
                      "~/Content/tabbedPanels.css"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}
