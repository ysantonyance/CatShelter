using CatShelter.Data;
using CatShelter.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CatShelter.Controllers
{
    public class CatCaresController : Controller
    {
        private readonly ApplicationDbContext _context;

        // конструктор на контролера с инжектиран контекст на базата
        public CatCaresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CatCares
        // извежда списък с всички грижи за котки включително информация за котката, грижата и потребителя
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CatCare.Include(c => c.Care).Include(c => c.Cat).Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CatCares/Details/5
        // извежда детайли за конкретна грижа за котка по id
        // включва информация за котката и породата, грижата и потребителя
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catCare = await _context.CatCare
                .Include(cc => cc.Cat)
                    .ThenInclude(c => c.Breed)
                .Include(cc => cc.Care)
                .Include(cc => cc.User)
                .FirstOrDefaultAsync(cc => cc.Id == id);
            if (catCare == null)
            {
                return NotFound();
            }

            return View(catCare);
        }

        // GET: CatCares/Create
        // показва форма за създаване на нова грижа за котка
        // извлича списъци с налични грижи, котки и потребители
        public IActionResult Create()
        {
            ViewBag.Cares = _context.Care.ToList(); 
            ViewBag.Cats = _context.Cat.Include(c => c.Breed).ToList();
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        // POST: CatCares/Create
        // обработва POST заявка за създаване на нова грижа за котка
        // задава текущия потребител, маркира като не удовлетворена и валидира модела
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CareId,Price,Id")] CatCare catCare)
        {
            catCare.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            catCare.IsSatisfied = false;
            ModelState.Remove("UserId");
            ModelState.Remove("IsSatisfied");

            // DEBUG - remove after fixing
            foreach (var key in ModelState.Keys)
            {
                foreach (var error in ModelState[key].Errors)
                {
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(catCare);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Cares = _context.Care.ToList();
            ViewBag.Cats = _context.Cat.Include(c => c.Breed).ToList();
            return View(catCare);
        }



        // GET: CatCares/Edit/5
        // показва форма за редакция на съществуваща грижа за котка
        // попълва select списъци с грижи, котки и потребители
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catCare = await _context.CatCare.FindAsync(id);
            if (catCare == null)
            {
                return NotFound();
            }
            ViewData["CareId"] = new SelectList(_context.Care, "Id", "CareName", catCare.CareId);
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Name", catCare.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", catCare.UserId);
            return View(catCare);
        }

        // POST: CatCares/Edit/5
        // обработва post заявка за редакция на грижа за котка
        // обновява полетата, ако моделът е валиден
        // само администратор може да промени IsSatisfied
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CatId,CareId,IsSatisfied,Price,UserId,Id")] CatCare catCare)
        {
            if (id != catCare.Id)
            {
                return NotFound();
            }

            var catCareInDb = await _context.CatCare.FindAsync(id);
            if (catCareInDb == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    catCareInDb.CatId = catCare.CatId;
                    catCareInDb.CareId = catCare.CareId;
                    catCareInDb.Price = catCare.Price;
                    catCareInDb.UserId = catCare.UserId;

                    if (User.IsInRole("Admin"))
                    {
                        catCareInDb.IsSatisfied = catCare.IsSatisfied;
                    }

                    _context.Update(catCareInDb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CatCareExists(catCare.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["CareId"] = new SelectList(_context.Care, "Id", "CareName", catCare.CareId);
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Name", catCare.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", catCare.UserId);
            return View(catCare);
        }

        // GET: CatCares/Delete/5
        // показва потвърждение за изтриване на грижа за котка
        // включва информация за котката, породата, грижата и потребителя
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var catCare = await _context.CatCare
                .Include(cc => cc.Cat)
                    .ThenInclude(c => c.Breed)  
                .Include(cc => cc.Care)         
                .Include(cc => cc.User)         
                .FirstOrDefaultAsync(cc => cc.Id == id);
            if (catCare == null)
            {
                return NotFound();
            }

            return View(catCare);
        }

        // POST: CatCares/Delete/5
        // обработва post заявка за изтриване на грижа за котка
        // премахва записа от базата
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var catCare = await _context.CatCare.FindAsync(id);
            if (catCare != null)
            {
                _context.CatCare.Remove(catCare);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // проверява дали дадена грижа за котка съществува по id
        private bool CatCareExists(int id)
        {
            return _context.CatCare.Any(e => e.Id == id);
        }

        // GET: CatCares/Donate
        // показва форма за дарение - с или без избор на котка
        public async Task<IActionResult> Donate()
        {
            ViewBag.Cares = await _context.Care.ToListAsync();
            ViewBag.UnhealthyCats = await _context.Cat
                .Include(c => c.Breed)
                .Where(c => !c.IsHealthy)
                .ToListAsync();

            return View();
        }

        // POST: CatCares/Donate
        // обработва дарение - ако няма избрана котка, избира тази с най-малко дарения
        // ако никоя няма дарения, избира случайна
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate(int? catId, int careId, decimal price)
        {
            if (price <= 0)
            {
                ModelState.AddModelError("", "Amount must be greater than 0.");
                ViewBag.Cares = await _context.Care.ToListAsync();
                ViewBag.UnhealthyCats = await _context.Cat
                    .Include(c => c.Breed)
                    .Where(c => !c.IsHealthy)
                    .ToListAsync();
                return View();
            }

            // ако няма избрана котка, намираме най-нуждаещата се
            if (catId == null)
            {
                var unhealthyCats = await _context.Cat
                    .Where(c => !c.IsHealthy)
                    .ToListAsync();

                if (!unhealthyCats.Any())
                {
                    ModelState.AddModelError("", "There are no unhealthy cats to donate to right now.");
                    ViewBag.Cares = await _context.Care.ToListAsync();
                    ViewBag.UnhealthyCats = new List<Cat>();
                    return View();
                }

                // броим даренията за всяка котка
                var catDonationCounts = await _context.CatCare
                    .GroupBy(cc => cc.CatId)
                    .Select(g => new { CatId = g.Key, Count = g.Count() })
                    .ToListAsync();

                // котки без нито едно дарение
                var catsWithNoDonations = unhealthyCats
                    .Where(c => !catDonationCounts.Any(d => d.CatId == c.Id))
                    .ToList();

                if (catsWithNoDonations.Any())
                {
                    // случайна котка без дарения
                    var random = new Random();
                    catId = catsWithNoDonations[random.Next(catsWithNoDonations.Count)].Id;
                }
                else
                {
                    // котката с най-малко дарения
                    var minCount = catDonationCounts
                        .Where(d => unhealthyCats.Any(c => c.Id == d.CatId))
                        .Min(d => d.Count);

                    catId = catDonationCounts
                        .Where(d => unhealthyCats.Any(c => c.Id == d.CatId) && d.Count == minCount)
                        .First().CatId;
                }
            }

            var catCare = new CatCare
            {
                CatId = catId.Value,
                CareId = careId,
                Price = price,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                IsSatisfied = false
            };

            _context.Add(catCare);
            await _context.SaveChangesAsync();

            // запазваме info за confirmation страницата
            var cat = await _context.Cat.FindAsync(catId);
            var care = await _context.Care.FindAsync(careId);
            TempData["DonationCat"] = cat?.Name;
            TempData["DonationCare"] = care?.CareName;
            TempData["DonationAmount"] = price.ToString("F2");

            return RedirectToAction(nameof(DonationConfirmation));
        }

        // GET: CatCares/DonationConfirmation
        public IActionResult DonationConfirmation()
        {
            return View();
        }
    }
}
