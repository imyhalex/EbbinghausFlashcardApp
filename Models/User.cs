using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace EbbinghausFlashcardApp.Models
{
    public class User : IdentityUser
    {
        public ICollection<FlashcardSet> FlashcardSets { get; set; }
    }
}
