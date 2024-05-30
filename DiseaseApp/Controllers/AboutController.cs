using Microsoft.AspNetCore.Mvc;

namespace DiseaseApp.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}