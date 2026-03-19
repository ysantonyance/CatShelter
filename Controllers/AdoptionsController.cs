using CatShelter.Data;
using CatShelter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
    public class AdoptionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdoptionsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Adoptions
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Adoption.Include(a => a.Cat).Include(a => a.User).ToListAsync());
            }

            var userId = _userManager.GetUserId(User);

            var userAdoptions = await _context.Adoption
                .Where(a => a.UserId == userId)
                .Include(a => a.Cat)
                .ToListAsync();

            return View(userAdoptions);
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
            ViewData["CatId"] = new SelectList(
                _context.Cat.Where(c => !c.IsAdopted),
                "Id",
                "Name"
            );
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Adoptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,AdoptionDate")] Adoption adoption)
        {
            adoption.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ModelState.Remove("UserId");
            adoption.Status = ApplicationStatus.Pending;

            if (ModelState.IsValid)
            {
                _context.Add(adoption);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Name", adoption.CatId);
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
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Name", adoption.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", adoption.UserId);
            return View(adoption);
        }

        // POST: Adoptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,CatId,AdoptionDate,Status,Id")] Adoption adoption)
        {
            if (adoption.Status == ApplicationStatus.Approved)
            {
                var cat = await _context.Cat.FindAsync(adoption.CatId);
                cat.IsAdopted = true; 
            }

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
            ViewData["CatId"] = new SelectList(_context.Cat, "Id", "Name", adoption.CatId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", adoption.UserId);
            return View(adoption);
        }

        // GET: Adoptions/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var adoption = await _context.Adoption.FindAsync(id);

            if (adoption == null) return NotFound();

            adoption.Status = ApplicationStatus.Approved;

            var cat = await _context.Cat.FindAsync(adoption.CatId);
            cat.IsAdopted = true;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var adoption = await _context.Adoption.FindAsync(id);

            if (adoption == null) return NotFound();

            adoption.Status = ApplicationStatus.Denied;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
