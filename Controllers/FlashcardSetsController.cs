using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
/*
 * Acknoledgement: 
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-8.0
 * @reference: https://stackoverflow.com/questions/57432387/understanding-task-vs-taskt-as-a-return-type
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/system.datetime.utcnow?view=net-8.0
 */
namespace EbbinghausFlashcardApp.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class FlashcardSetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FlashcardSetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // display flashcardsets in the at FalshcardSets/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var flashcardSets = await _context.FlashcardSets.Where(f => f.UserId == UserId).ToListAsync();
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
            if (ModelState.IsValid)
            {
                // optional image for set
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    var filePath = Path.Combine(uploadFolder, imageFile.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }
                    flashcardSet.ImagePath = "/images/" + imageFile.FileName;
                }

                flashcardSet.UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                flashcardSet.CreatedDate = DateTime.UtcNow;
                // once created, the flashcard set will be marked as not reviewed
                flashcardSet.ReviewInterval = -1;
                flashcardSet.NextReviewDate = DateTime.UtcNow;

                if (flashcardSet.Flashcards == null)
                {
                    flashcardSet.Flashcards = new List<Flashcard>();
                }

                _context.Add(flashcardSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(flashcardSet);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var flashcardSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(m => m.Id == id);
            if (flashcardSet == null)
                return NotFound();
            return View(flashcardSet);
        }

        // method to edit exsiting flashcard set (GET)
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var flashcarSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(m => m.Id == id);
            if (flashcarSet == null)
                return NotFound();
            return View(flashcarSet);
        }

        // method to edit existing flashcar set (POST) - from "The PutTodoItem method" in mircosoft tutorial
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlashcardSet flashcardSet)
        {
            if (id != flashcardSet.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Attach(flashcardSet);
                    _context.Entry(flashcardSet).State = EntityState.Modified;

                    // if new flashcard have been createdm save changes to the database
                    foreach (var flashcard in flashcardSet.Flashcards)
                    {
                        if (flashcard.Id == 0)
                        {
                            flashcard.FlashcardSetId = flashcardSet.Id;
                            _context.Flashcards.Add(flashcard);
                        }
                        else
                            _context.Entry(flashcard).State = EntityState.Modified;
                    }
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FlashcardSetExists(id))
                        return NotFound();
                    else
                        throw;
                }
            }
            return View(flashcardSet);
        }

        // helper function: check if flashcard set exists
        private bool FlashcardSetExists(int id)
        {
            return _context.FlashcardSets.All(e => e.Id == id);
        }
    }
}
