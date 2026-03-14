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
                .Include(c => c.Care)
                .Include(c => c.Cat)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (catCare == null)
            {
                return NotFound();
            }

            return View(catCare);
        }

        // GET: CatCares/Create
        public IActionResult Create()
        {
            ViewData["CareId"] = new SelectList(_context.Care, "Id", "Id");
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CatCares/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CareId,IsSatisfied,Price,UserId,Id")] CatCare catCare)
        {
            if (ModelState.IsValid)
            {
                _context.Add(catCare);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CareId"] = new SelectList(_context.Care, "Id", "Id", catCare.CareId);
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id", catCare.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", catCare.UserId);
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
            ViewData["CareId"] = new SelectList(_context.Care, "Id", "Id", catCare.CareId);
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id", catCare.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", catCare.UserId);
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

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(catCare);
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
            ViewData["CareId"] = new SelectList(_context.Care, "Id", "Id", catCare.CareId);
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Id", catCare.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", catCare.UserId);
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
                .Include(c => c.Care)
                .Include(c => c.Cat)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
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
