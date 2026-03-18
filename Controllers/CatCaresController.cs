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

        public CatCaresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CatCares
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CatCare.Include(c => c.Care).Include(c => c.Cat).Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CatCares/Details/5
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
        public IActionResult Create()
        {
            ViewBag.Cares = _context.Care.ToList(); 
            ViewBag.Cats = _context.Cat.Include(c => c.Breed).ToList();
            ViewBag.Users = _context.Users.ToList();
            return View();
        }

        // POST: CatCares/Create
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

        private bool CatCareExists(int id)
        {
            return _context.CatCare.Any(e => e.Id == id);
        }
    }
}
