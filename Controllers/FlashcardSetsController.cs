using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using EbbinghausFlashcardApp.Hubs;

/*
 * Acknoledgement: this code follows the Microsoft tutorials or documentations below
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-8.0
 * @reference: https://stackoverflow.com/questions/57432387/understanding-task-vs-taskt-as-a-return-type
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/system.datetime.utcnow?view=net-8.0
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment?view=aspnetcore-8.0
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.any?view=net-8.0
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/data/ef-mvc/crud?view=aspnetcore-8.0
 */
namespace EbbinghausFlashcardApp.Controllers
{
    [Authorize]
    public class FlashcardSetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IHubContext<NotificationHub> _hubContext;

        public FlashcardSetsController(ApplicationDbContext context, IWebHostEnvironment environment, IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _environment = environment;
            _hubContext = hubContext;
        }

        /* this part is for flashcard sets rendering and crud implementations */

        // display flashcardsets in the at FalshcardSets/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var flashcardSets = await _context.FlashcardSets
                .Where(f => f.UserId == UserId).Include(f => f.Flashcards).ToListAsync();
            return View(flashcardSets);
        }

        // a create page for flashcardsets
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // create a new flashcard set (POST), and the flashcard set will be marked as not reviewed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FlashcardSet flashcardSet, IFormFile imageFile)
        {
            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadFolder = Path.Combine(_environment.WebRootPath, "images");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    var filePath = Path.Combine(uploadFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    flashcardSet.ImagePath = "/images/" + fileName;
                }
                // set the user id and dates
                flashcardSet.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                flashcardSet.CreatedDate = DateTime.UtcNow;
                flashcardSet.ReviewInterval = -1;
                flashcardSet.NextReviewDate = DateTime.UtcNow;

                // if no flashcards are provided in the form, this line will create an empty list for flashcardSet.Flashcards
                if (flashcardSet.Flashcards == null)
                    flashcardSet.Flashcards = new List<Flashcard>();

                // add flashcard set to the database
                await _context.FlashcardSets.AddAsync(flashcardSet);
                await _context.SaveChangesAsync();
                // add the new flashcard to the review list
                await _hubContext.Clients.User(flashcardSet.UserId)
                    .SendAsync("AddFlashcardSet", flashcardSet.Id, flashcardSet.Name);
                await _hubContext.Clients.User(flashcardSet.UserId)
                    .SendAsync("ReceiveMessage", "System", $"Flashcard set '{flashcardSet.Name}' is now ready for review.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    Console.WriteLine("Inner exception: " + e.InnerException.Message);
                Console.WriteLine("Exception: " + e.Message);
                ModelState.AddModelError("", "Error: " + e.Message + (e.InnerException != null ? " - Inner exception: " + e.InnerException.Message : ""));
                return View(flashcardSet);
            }
        }

        // this method for details page's rendering purpose
        public async Task<IActionResult> Details(int id)
        {
            var flashcardSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(m => m.Id == id);
            if (flashcardSet == null)
                return NotFound();
            return View(flashcardSet);
        }

        // method to delete a flashcard set
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteFlashcardSet(int id)
        {
            var flashcardSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(m => m.Id == id);
            if (flashcardSet == null)
                return NotFound();

            // first remove all flashcards in the flashcard set
            _context.Flashcards.RemoveRange(flashcardSet.Flashcards);
            // then remove the flashcard set
            _context.FlashcardSets.Remove(flashcardSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSetName(int id, string name)
        {
            var flashcardSet = await _context.FlashcardSets.FindAsync(id);
            if (flashcardSet == null)
                return NotFound();

            flashcardSet.Name = name;
            _context.Update(flashcardSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSetDescription(int id, string description)
        {
            var flashcardSet = await _context.FlashcardSets.FindAsync(id);
            if (flashcardSet == null)
                return NotFound();

            flashcardSet.Description = description;
            _context.Update(flashcardSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }

        /* this part is for flashcard controllers - CRUD operation for flashcards */

        // method to add a flashcard
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFlashcard(int flashcardSetId, List<Flashcard> flashcards)
        {
            if (flashcards == null || !flashcards.Any())
                return RedirectToAction(nameof(Details), new { id = flashcardSetId });
            var flashcardSet = await _context.FlashcardSets.FindAsync(flashcardSetId);
            if (flashcardSet == null)
                return NotFound();
            // add each new flashcard to the database
            foreach (var flashcard in flashcards)
            {
                if (!string.IsNullOrEmpty(flashcard.Term) && !string.IsNullOrEmpty(flashcard.Definition))
                {
                    flashcard.FlashcardSetId = flashcardSetId;
                    _context.Flashcards.AddAsync(flashcard);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = flashcardSetId });
        }

        // method to edit a falshcard
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFlashcard(int id, string term, string definition)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null)
                return NotFound();
            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(definition))
                return RedirectToAction(nameof(Details), new { id = flashcard.FlashcardSetId });
            
            flashcard.Term = term;
            flashcard.Definition = definition;
            _context.Update(flashcard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = flashcard.FlashcardSetId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteFlashcard(int id)
        {
            var flashcard = await _context.Flashcards.FindAsync(id);
            if (flashcard == null)
                return NotFound();
            
            var flashcardSetId = flashcard.FlashcardSetId;
            _context.Flashcards.Remove(flashcard);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = flashcardSetId });
        }

        // method to delete all flashcard within a flashcard set
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllFlashcards(int flashcardSetId)
        {
            var flashcarSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(f => f.Id == flashcardSetId);
            if (flashcarSet == null)
                return NotFound();
            
            // perform removal of all flashcards
            _context.Flashcards.RemoveRange(flashcarSet.Flashcards);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = flashcardSetId });
        }

        /* all helper function goes here */
        // helper function: check if flashcard set exists
        private bool FlashcardSetExists(int id)
        {
            return _context.FlashcardSets.All(e => e.Id == id);
        }
    }
}
