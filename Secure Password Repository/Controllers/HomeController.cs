using System.Web.Mvc;

namespace Secure_Password_Repository.Controllers
{
    #if !DEBUG
    [RequireHttps] //apply to all actions in controller
    #endif
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}