using DiseaseApp.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace DiseaseApp.Controllers
{
    public class TeamController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}