using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CatShelter.Data;
using CatShelter.Models;

namespace CatShelter.Controllers
{
    public class CatsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // конструктор на контролера с инжектиран контекст на базата
        public CatsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cats
        // извежда списък с всички котки включително породата им
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Cat.Include(c => c.Breed);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Cats/Details/5
        // извежда детайли за конкретна котка по id включително породата
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cat = await _context.Cat
                .Include(c => c.Breed)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cat == null)
            {
                return NotFound();
            }

            return View(cat);
        }

        // GET: Cats/Create
        // показва форма за създаване на нова котка
        // извлича списък с породи за селект
        public IActionResult Create()
        {
            ViewBag.Breeds = _context.Breed.ToList();
            return View();
        }

        // POST: Cats/Create
        // обработва post заявка за създаване на котка
        // добавя котката ако моделът е валиден
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,BirthDate,Kg,Img,BreedId,IsAdopted,IsHealthy,Description, Id")] Cat cat)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Breeds = _context.Breed.ToList(); 
            return View(cat);
        }


        // GET: Cats/Edit/5
        // показва форма за редакция на съществуваща котка
        // извлича списък с породи за селект
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cat = await _context.Cat.FindAsync(id);
            if (cat == null)
            {
                return NotFound();
            }
            ViewBag.Breeds = _context.Breed.ToList();
            return View(cat);
        }

        // POST: Cats/Edit/5
        // обработва post заявка за редакция на котка
        // обновява данните ако моделът е валиден
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Cat cat, IFormFile? imageFile)
        {
            if (id != cat.Id)
                return NotFound();

            var existingCat = await _context.Cat.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

            if (imageFile == null)
            {
                ModelState.Remove("Img");
                cat.Img = existingCat?.Img;
            }
            else if (imageFile.Length > 0)
            {
                var allowed = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var ext = Path.GetExtension(imageFile.FileName).ToLowerInvariant();
                if (!allowed.Contains(ext))
                {
                    ModelState.AddModelError("Img", "Only image files are allowed.");
                }
            }

            if (!ModelState.IsValid)
            {
                foreach (var kvp in ModelState.Where(x => x.Value.Errors.Any()))
                    Console.WriteLine($"INVALID FIELD: {kvp.Key}");

                ViewBag.Breeds = await _context.Breed.ToListAsync();
                return View(cat);
            }

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    try
                    {
                        string fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "cats");
                        Directory.CreateDirectory(uploadPath);

                        string filePath = Path.Combine(uploadPath, fileName);

                        using var stream = new FileStream(filePath, FileMode.Create);
                        await imageFile.CopyToAsync(stream);

                        if (!string.IsNullOrEmpty(existingCat?.Img))
                        {
                            string oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                                existingCat.Img.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                            if (System.IO.File.Exists(oldPath))
                                System.IO.File.Delete(oldPath);
                        }

                        cat.Img = "/uploads/cats/" + fileName;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Image upload failed: " + ex.Message);
                        ViewBag.Breeds = await _context.Breed.ToListAsync();
                        return View(cat);
                    }
                }

                _context.Update(cat);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CatExists(cat.Id)) return NotFound();
                throw;
            }
        }

        // GET: Cats/Delete/5
        // показва потвърждение за изтриване на котка
        // включва информация за породата
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cat = await _context.Cat
                .Include(c => c.Breed)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cat == null)
            {
                return NotFound();
            }

            return View(cat);
        }

        // POST: Cats/Delete/5
        // обработва post заявка за изтриване на котка
        // премахва котката от базата
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cat = await _context.Cat.FindAsync(id);
            if (cat != null)
            {
                _context.Cat.Remove(cat);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // проверява дали дадена котка съществува по id
        private bool CatExists(int id)
        {
            return _context.Cat.Any(e => e.Id == id);
        }

        // извежда списък с осиновени и налични котки
        // използва информация от таблицата с осиновявания
        public async Task<IActionResult> AdoptionStatus()
        {
            var adoptedCatIds = await _context.Adoption
                .Select(a => a.CatId)
                .ToListAsync();

            var adoptedCats = await _context.Cat
                .Where(c => adoptedCatIds.Contains(c.Id))
                .Include(c => c.Breed)
                .ToListAsync();

            var availableCats = await _context.Cat
                .Where(c => !adoptedCatIds.Contains(c.Id))
                .Include(c => c.Breed)
                .ToListAsync();

            ViewBag.AdoptedCats = adoptedCats;
            ViewBag.AvailableCats = availableCats;

            return View();
        }
    }
}
