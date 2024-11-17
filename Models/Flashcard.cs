using System.ComponentModel.DataAnnotations;

namespace EbbinghausFlashcardApp.Models
{
    public class Flashcard
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Term is required.")]
        public string Term { get; set; }

        [Required(ErrorMessage = "Definition is required.")]
        public string Definition { get; set; }
        public bool IsFamiliar { get; set; } // Indicates if the user marked the flashcard as familiar

        // this is a nullable field
        public string? ImagePath { get; set; } // Path to the uploaded image

        // Foreign key to FlashcardSet
        public int FlashcardSetId { get; set; }
        public FlashcardSet FlashcardSet { get; set; }
    }
}
