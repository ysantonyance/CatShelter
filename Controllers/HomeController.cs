using CatShelter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CatShelter.Controllers
{
    public class HomeController : Controller
    {
        // показва началната страница
        public IActionResult Index()
        {
            return View();
        }

        // показва страницата за информация за сайта
        public IActionResult About()
        {
            return View();
        }

        // показва страницата за грешка
        // не кешира отговора
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
