using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Libripolis.Data;
using Libripolis.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Libripolis.Controllers
{
    public class BorrowsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BorrowsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Borrows


        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userBorrows = await _context.Borrow
                .Include(b => b.user)
                .Include(b => b.Book)
                .Where(b => b.user.Id == userId)
                .ToListAsync();

            return View(userBorrows);
        }
       

        // GET: Borrows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Borrow == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow
                .Include(b => b.Book)
                .Include(b => b.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // GET: Borrows/Create
        public IActionResult Create(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdź, czy użytkownik już ma wypożyczoną tę książkę z ważnym terminem wypożyczenia
            var existingBorrow = _context.Borrow
                .Where(b => b.userId == userId && b.BookId == bookId && b.End > DateTime.Now)
                .FirstOrDefault();

            if (existingBorrow != null)
            {
                // Użytkownik już ma wypożyczoną tę książkę z ważnym terminem wypożyczenia
                
                TempData["ErrorMessage"] = "Nie możesz wypożyczyć tej samej książki, ponieważ masz ją już wypożyczoną z ważnym terminem.";
                return RedirectToAction(nameof(Index));
            }

           
            var borrow = new Borrow
            {
                userId = userId,
                BookId = bookId,
                Start = DateTime.Now, 
                End = DateTime.Now
            };

            
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title");
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Email", borrow.userId);
            return View(borrow);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Member")]
        public async Task<IActionResult> Create([Bind("Id,BookId,userId,Start,End")] Borrow borrow)
        {
            if (ModelState.IsValid)
            {
                // Sprawdź, czy użytkownik już ma wypożyczoną tę książkę z ważnym terminem wypożyczenia
                var existingBorrow = _context.Borrow
                    .Where(b => b.userId == borrow.userId && b.BookId == borrow.BookId && b.End > DateTime.Now)
                    .FirstOrDefault();

                if (existingBorrow != null)
                {
                    // Użytkownik już ma wypożyczoną tę książkę z ważnym terminem wypożyczenia
                    TempData["ErrorMessage"] = "Nie możesz wypożyczyć tej samej książki, ponieważ masz ją już wypożyczoną z ważnym terminem.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Add(borrow);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", borrow.BookId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", borrow.userId);
            return View(borrow);
        }

        // GET: Borrows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Borrow == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow.FindAsync(id);
            if (borrow == null)
            {
                return NotFound();
            }

           
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Title", borrow.BookId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Email", borrow.userId);

         
            return View(borrow);
        }

        // POST: Borrows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,userId,Start,End")] Borrow borrow)
        {
            if (id != borrow.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(borrow);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BorrowExists(borrow.Id))
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
            ViewData["BookId"] = new SelectList(_context.Book, "Id", "Id", borrow.BookId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", borrow.userId);
            return View(borrow);
        }

        // GET: Borrows/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Borrow == null)
            {
                return NotFound();
            }

            var borrow = await _context.Borrow
                .Include(b => b.Book)
                .Include(b => b.user)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (borrow == null)
            {
                return NotFound();
            }

            return View(borrow);
        }

        // POST: Borrows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Borrow == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Borrow'  is null.");
            }
            var borrow = await _context.Borrow.FindAsync(id);
            if (borrow != null)
            {
                _context.Borrow.Remove(borrow);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BorrowExists(int id)
        {
            return (_context.Borrow?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> allBorows()
        {
            var allBorrows = await _context.Borrow
                .Include(b => b.user)
                .Include(b => b.Book)
                .ToListAsync();

            return View(allBorrows);
        }

    }
}
