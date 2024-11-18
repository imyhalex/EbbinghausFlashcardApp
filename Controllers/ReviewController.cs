﻿using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

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

        // Intervals in hours after a review:
        // 0 -> initial learning
        // 20 minutes -> roughly 0.33 hours
        // 1 hour -> 1
        // 6 houds -> 6
        // 12 hours -> 12
        // 1 day -> 24
        // 2 days -> 48, 4 days -> 96, etc all the way to 720 hours (30 days)
        private int[] intervals = { 0, 1, 6, 12, 24, 48, 96, 168, 360, 720 };

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

        /* this part is for ebbinghaus logic */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteReview(int flashcardSetId)
        {
            var flashcardSet = await _context.FlashcardSets.FindAsync(flashcardSetId);
            if (flashcardSet == null)
                return NotFound();

            // for the next review interval
            var currentStage = CalculateReviewStage(flashcardSet.CreatedDate, DateTime.UtcNow);
            int nextInterval = GetNextInterval(currentStage + 1);

            if (nextInterval == 0)
                flashcardSet.NextReviewDate = DateTime.UtcNow.AddMinutes(20); // test the app by changing the interval to 1 minute
            else
                flashcardSet.NextReviewDate = DateTime.UtcNow.AddHours(nextInterval);

            _context.Update(flashcardSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), "FlashcardSets", new { id = flashcardSetId});
        }

        // helper methods to increment review count by "1"
        private int CalculateReviewStage(DateTime createdDate, DateTime currentDate)
        {
            TimeSpan elapsedTime = currentDate - createdDate;
            // anchor the stage currently in
            for (int i = intervals.Length - 1; i >= 0; i--)
            {
                if(elapsedTime.TotalHours >= intervals[i])
                    return i;
            }
            return 0;
        }

        private int GetNextInterval(int reviewStage)
        {
            return reviewStage < intervals.Length ? intervals[reviewStage] : 720; // the cap is 30 days
        }
    }
}
