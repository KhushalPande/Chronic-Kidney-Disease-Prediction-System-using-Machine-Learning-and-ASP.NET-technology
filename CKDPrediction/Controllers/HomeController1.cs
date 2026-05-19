using Microsoft.AspNetCore.Mvc;

namespace CKDPrediction.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
