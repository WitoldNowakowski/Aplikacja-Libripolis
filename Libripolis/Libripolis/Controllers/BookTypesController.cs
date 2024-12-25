using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Libripolis.Data;
using Libripolis.Models;
using Microsoft.AspNetCore.Authorization;

namespace Libripolis.Controllers

{
    [Authorize(Roles = "Admin")]
    public class BookTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BookTypes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BookType.Include(b => b.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BookTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BookType == null)
            {
                return NotFound();
            }

            var bookType = await _context.BookType
                .Include(b => b.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookType == null)
            {
                return NotFound();
            }

            return View(bookType);
        }

        // GET: BookTypes/Create
        public IActionResult Create()
        {
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: BookTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,userId")] BookType bookType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bookType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", bookType.userId);
            return View(bookType);
        }

        // GET: BookTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BookType == null)
            {
                return NotFound();
            }

            var bookType = await _context.BookType.FindAsync(id);
            if (bookType == null)
            {
                return NotFound();
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Email", bookType.userId);
            return View(bookType);
        }

        // POST: BookTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,userId")] BookType bookType)
        {
            if (id != bookType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bookType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookTypeExists(bookType.Id))
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
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", bookType.userId);
            return View(bookType);
        }

        // GET: BookTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BookType == null)
            {
                return NotFound();
            }

            var bookType = await _context.BookType
                .Include(b => b.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bookType == null)
            {
                return NotFound();
            }

            return View(bookType);
        }

        // POST: BookTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BookType == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BookType'  is null.");
            }
            var bookType = await _context.BookType.FindAsync(id);
            if (bookType != null)
            {
                _context.BookType.Remove(bookType);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookTypeExists(int id)
        {
          return (_context.BookType?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
