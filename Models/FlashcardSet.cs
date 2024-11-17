using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EbbinghausFlashcardApp.Models
{
    public class FlashcardSet
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReviewInterval { get; set; } // Reviewed spots in 20minutes, 1 hour, half a day, 1 day, 2 days, 4 days, 7 days, 15 days, 30 days (reset and restart a new cycle after 30 days)
        public DateTime NextReviewDate { get; set; }

        // caught by exception in Controllers/FlashcarSets.cs Create(): this should be nullable, else Set cannot be insert successfully
        public string? ImagePath { get; set; } // Path to the uploaded image

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
        // Navigation property
        public ICollection<Flashcard> Flashcards { get; set; }
    }
}
