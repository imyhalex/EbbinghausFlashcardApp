using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EbbinghausFlashcardApp.Models
{
    public class FlashcardSet
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ReviewInterval { get; set; } // Reviewed spots in 20minutes, 1 hour, half a day, 1 day, 2 days, 4 days, 7 days, 15 days, 30 days (reset and restart a new cycle after 30 days)
        public DateTime NextReviewDate { get; set; }
        public string ImagePath { get; set; } // Path to the uploaded image

        [ForeignKey("User")]
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
        // Navigation property
        public ICollection<Flashcard> Flashcards { get; set; }
    }
}
