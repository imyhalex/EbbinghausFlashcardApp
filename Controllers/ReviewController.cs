using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Models;

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

        public async Task<IActionResult> Review(int flashcardSetId)
        {
            var flashcardSet = await _context.FlashcardSets
                .Include(f => f.Flashcards)
                .FirstOrDefaultAsync(m => m.Id == flashcardSetId);

            if (flashcardSet == null || flashcardSet.UserId != User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value)
            {
                return NotFound();
            }

            var flashcardsToReview = flashcardSet.Flashcards.Where(f => f.IsFamiliar == false).ToList();

            if (flashcardsToReview.Count == 0)
            {
                ViewBag.Message = "No flashcards to review!";
                return View("NoReview");
            }

            return View(flashcardsToReview.FirstOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> Review(int id, bool isFamiliar)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);

            if (flashcard == null)
            {
                return NotFound();
            }

            flashcard.IsFamiliar = isFamiliar;
            await _context.SaveChangesAsync();

            return RedirectToAction("Review", new { flashcardSetId = flashcard.FlashcardSetId });
        }
    }
}
