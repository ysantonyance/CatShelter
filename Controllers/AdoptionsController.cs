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
    public class AdoptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdoptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Adoptions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Adoption.Include(a => a.Cat).Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Adoptions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adoption = await _context.Adoption
                .Include(a => a.Cat)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adoption == null)
            {
                return NotFound();
            }

            return View(adoption);
        }

        // GET: Adoptions/Create
        public IActionResult Create()
        {
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Adoptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,CatId,AdoptionDate,Status,Id")] Adoption adoption)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adoption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id", adoption.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", adoption.UserId);
            return View(adoption);
        }

        // GET: Adoptions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adoption = await _context.Adoption.FindAsync(id);
            if (adoption == null)
            {
                return NotFound();
            }
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id", adoption.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", adoption.UserId);
            return View(adoption);
        }

        // POST: Adoptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,CatId,AdoptionDate,Status,Id")] Adoption adoption)
        {
            if (id != adoption.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adoption);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdoptionExists(adoption.Id))
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
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id", adoption.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", adoption.UserId);
            return View(adoption);
        }

        // GET: Adoptions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adoption = await _context.Adoption
                .Include(a => a.Cat)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (adoption == null)
            {
                return NotFound();
            }

            return View(adoption);
        }

        // POST: Adoptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adoption = await _context.Adoption.FindAsync(id);
            if (adoption != null)
            {
                _context.Adoption.Remove(adoption);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdoptionExists(int id)
        {
            return _context.Adoption.Any(e => e.Id == id);
        }
    }
}
