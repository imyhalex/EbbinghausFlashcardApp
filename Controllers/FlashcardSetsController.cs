using EbbinghausFlashcardApp.Data;
using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
/*
 * Acknoledgement: this code follows the Microsoft tutorials or documentations below
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-controller?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-8.0
 * @reference: https://stackoverflow.com/questions/57432387/understanding-task-vs-taskt-as-a-return-type
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/system.datetime.utcnow?view=net-8.0
 * @reference: https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.hosting.iwebhostenvironment?view=aspnetcore-8.0
 */
namespace EbbinghausFlashcardApp.Controllers
{
    [Authorize]
    public class FlashcardSetsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public FlashcardSetsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

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
            // debug
            Console.WriteLine("Create POST action set");
            Console.WriteLine($"flashcardSet: {flashcardSet?.Name}");
            Console.WriteLine($"FlashcardSet Description: {flashcardSet?.Description}");
            Console.WriteLine($"Image File Present: {imageFile != null}");
            Console.WriteLine($"Flashcards Count: {flashcardSet?.Flashcards?.Count ?? 0}");
            try
            {
                if (ModelState.IsValid)
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

                    // if flashcard collection is null, initialize it
                    if (flashcardSet.Flashcards == null)
                        flashcardSet.Flashcards = new List<Flashcard>();

                    // add flashcard set to the database
                    await _context.FlashcardSets.AddAsync(flashcardSet);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(flashcardSet);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "Error: " + e.Message);
                return View(flashcardSet);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var flashcardSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(m => m.Id == id);
            if (flashcardSet == null)
                return NotFound();
            return View(flashcardSet);
        }

        // method to edit exsiting flashcard set (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var flashcarSet = await _context.FlashcardSets
                .Include(f => f.Flashcards).FirstOrDefaultAsync(m => m.Id == id);
            if (flashcarSet == null)
                return NotFound();
            return View(flashcarSet);
        }

        // method to edit existing flashcar set (POST) - from "The PutTodoItem method" in mircosoft tutorial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FlashcardSet flashcardSet)
        {
            if (id != flashcardSet.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(flashcardSet);

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
