using System.ComponentModel.DataAnnotations;

namespace EbbinghausFlashcardApp.Models
{
    public class Flashcard
    {
        public int Id { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public bool IsFamiliar { get; set; } // Indicates if the user marked the flashcard as familiar
        public string ImagePath { get; set; } // Path to the uploaded image

        // Foreign key to FlashcardSet
        public int FlashcardSetId { get; set; }
        public FlashcardSet FlashcardSet { get; set; }
    }
}
