using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CatShelter.Data;
using CatShelter.Models;
using Microsoft.AspNetCore.Authorization;

namespace CatShelter.Controllers
{
    public class BreedsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // конструктор на контролера с инжектиран контекст на базата
        public BreedsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Breeds
        // извежда списък с всички породи
        public async Task<IActionResult> Index(string? search)
        {
            var breeds = _context.Breed.Include(b => b.Cats).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                breeds = breeds.Where(b => b.Name.Contains(search));

            ViewBag.Search = search;
            return View(await breeds.ToListAsync());
        }

        // GET: Breeds/Details/5
        // извежда детайли за конкретна порода по id
        // ако породата не съществува връща NotFound
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var breed = await _context.Breed
                .FirstOrDefaultAsync(m => m.Id == id);
            if (breed == null)
            {
                return NotFound();
            }

            return View(breed);
        }

        // GET: Breeds/Create
        // показва форма за създаване на нова порода
        // достъпно само за администратори
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Breeds/Create
        // обработва post заявка за създаване на порода
        // добавя породата ако моделът е валиден
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,Description,Id")] Breed breed)
        {
            if (ModelState.IsValid)
            {
                _context.Add(breed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(breed);
        }

        // GET: Breeds/Edit/5
        // показва форма за редакция на съществуваща порода
        // достъпно само за администратори
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var breed = await _context.Breed.FindAsync(id);
            if (breed == null)
            {
                return NotFound();
            }
            return View(breed);
        }

        // POST: Breeds/Edit/5
        // обработва post заявка за редакция на порода
        // обновява данните ако моделът е валиден
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,Id")] Breed breed)
        {
            if (id != breed.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(breed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BreedExists(breed.Id))
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
            return View(breed);
        }

        // GET: Breeds/Delete/5
        // показва потвърждение за изтриване на порода
        // достъпно само за администратори
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var breed = await _context.Breed
                .FirstOrDefaultAsync(m => m.Id == id);
            if (breed == null)
            {
                return NotFound();
            }

            return View(breed);
        }

        // POST: Breeds/Delete/5
        // обработва POST заявка за изтриване на порода
        // премахва породата от базата ако съществува
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var breed = await _context.Breed.FindAsync(id);
            if (breed != null)
            {
                _context.Breed.Remove(breed);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // проверява дали дадена порода съществува по id
        private bool BreedExists(int id)
        {
            return _context.Breed.Any(e => e.Id == id);
        }
    }
}
