using CatShelter.Data;
using CatShelter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CatShelter.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // показва началната страница
        public async Task<IActionResult> Index()
        {
            ViewBag.CatCount = await _context.Cat.CountAsync();
            ViewBag.AdoptionCount = await _context.Adoption.CountAsync();
            ViewBag.CareSessionCount = await _context.CatCare.CountAsync();
            ViewBag.BreedCount = await _context.Breed.CountAsync();
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
