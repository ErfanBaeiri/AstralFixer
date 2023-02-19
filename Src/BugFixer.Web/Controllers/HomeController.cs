using Microsoft.AspNetCore.Mvc;

namespace BugFixer.Web.Controllers
{
    public class HomeController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}