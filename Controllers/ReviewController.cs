using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Hubs;
using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

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
        private readonly IHubContext<NotificationHub> _hubContext;

        // Intervals in hours after a review:
        // 0 -> initial learning
        // 20 minutes -> roughly 0.33 hours
        // 1 hour -> 1
        // 6 houds -> 6
        // 12 hours -> 12
        // 1 day -> 24
        // 2 days -> 48, 4 days -> 96, etc all the way to 720 hours (30 days)
        private TimeSpan[] intervals = new TimeSpan[]
        {
            TimeSpan.Zero,
            TimeSpan.FromMinutes(20),
            TimeSpan.FromHours(1),
            TimeSpan.FromHours(6),
            TimeSpan.FromHours(12),
            TimeSpan.FromDays(1),
            TimeSpan.FromDays(2),
            TimeSpan.FromDays(4),
            TimeSpan.FromDays(7),
            TimeSpan.FromDays(15),
            TimeSpan.FromDays(30)
        };

        public ReviewController(ApplicationDbContext context, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
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

            // get the total number of flashcards in the set
            var flashcardSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(f => f.Id == flashcardSetId);
            if (flashcardSet == null || !flashcardSet.Flashcards.Any())
                return NotFound();

            int totalCount = flashcardSet.Flashcards.Count;
            if (currentIndex + 1 >= totalCount)
                return View("ReviewComplete", flashcardSet);

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

            var currentStage = CalculateReviewStage(flashcardSet.CreatedDate, DateTime.UtcNow);
            var nextInterval = GetNextInterval(currentStage + 1);

            if (nextInterval.Equals(intervals.Last()))
            {
                // Reset the progress
                flashcardSet.NextReviewDate = DateTime.UtcNow.Add(intervals[1]); // 20 minutes
                await _hubContext.Clients.User(flashcardSet.UserId)
                    .SendAsync("ReceiveMessage", "System", $"Flashcard set '{flashcardSet.Name}' has been reset. The review process starts again from the beginning.");
            }
            else 
                flashcardSet.NextReviewDate = DateTime.UtcNow.Add(nextInterval);

            _context.Update(flashcardSet);
            await _context.SaveChangesAsync();

            // Format the interval for display
            string formattedInterval = FormatTimeSpan(nextInterval);

            // Send notifications to the user
            await _hubContext.Clients.User(flashcardSet.UserId)
                .SendAsync("RemoveFlashcardSet", flashcardSetId);
            await _hubContext.Clients.User(flashcardSet.UserId)
                .SendAsync("ReceiveMessage", "System", $"You have completed review of flashcard set '{flashcardSet.Name}'. The next review will be in approximately {formattedInterval}.");

            return RedirectToAction(nameof(FlashcardSetsController.Details), "FlashcardSets", new { id = flashcardSetId });
        }

        // helper methods to increment review count by "1"
        private int CalculateReviewStage(DateTime createdDate, DateTime currentDate)
        {
            TimeSpan elapsedTime = currentDate - createdDate;

            for (int i = intervals.Length - 1; i >= 0; i--)
            {
                if (elapsedTime >= intervals[i])
                    return i;
            }
            return 0;
        }

        private TimeSpan GetNextInterval(int reviewStage)
        {
            return reviewStage < intervals.Length ? intervals[reviewStage] : intervals.Last();
        }

        // trigger real-time review notification
        private async Task TriggerReviewNotification(int flashcardSetId, string flashcardSetName)
        {
            await _hubContext.Clients.All.SendAsync("AddFlashcardSet", flashcardSetId, flashcardSetName);
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 1)
                return "less than a minute";

            int days = timeSpan.Days;
            int hours = timeSpan.Hours;
            int minutes = timeSpan.Minutes;

            var parts = new List<string>();

            if (days > 0)
                parts.Add($"{days} day{(days > 1 ? "s" : "")}");
            if (hours > 0)
                parts.Add($"{hours} hour{(hours > 1 ? "s" : "")}");
            if (minutes > 0)
                parts.Add($"{minutes} minute{(minutes > 1 ? "s" : "")}");

            return string.Join(", ", parts);
        }
    }
}
