using System.Web.Mvc;

namespace TextToPdfClassic.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() => View();

        public ActionResult TryIt() => View();
    }
}
