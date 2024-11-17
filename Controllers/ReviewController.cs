using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

/*
 * Acknowledgement: This code is based on the following part: "Views in ASP.NET Core" from the following reference.
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/mvc/views/overview?view=aspnetcore-8.0 - "What is ViewData and ViewBag"
 */

namespace EbbinghausFlashcardApp.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult?> Review(int flashcardSetId, int currentIndex)
        {
            var flashcardSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(f => f.Id == flashcardSetId);
            if (flashcardSet == null || !flashcardSet.Flashcards.Any())
                return NotFound();

            if (currentIndex < 0)
                currentIndex = 0;
            if (currentIndex >= flashcardSet.Flashcards.Count)
                currentIndex = flashcardSet.Flashcards.Count - 1;

            ViewBag.CurrentIndex = currentIndex;
            ViewBag.TotalCount = flashcardSet.Flashcards.Count;

            return View(flashcardSet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkFlashcard(int flashcardSetId, int flashcardId, bool isFamiliar, int currentIndex)
        {
            var flashcard = await _context.Flashcards.FindAsync(flashcardId);
            if (flashcard == null)
                return NotFound();

            flashcard.IsFamiliar = isFamiliar;
            _context.Update(flashcard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Review), new { flashcardSetId, currentIndex = currentIndex + 1 });
        }
    }
}
