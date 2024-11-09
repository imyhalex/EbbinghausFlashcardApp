using EbbinghausFlashcardApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

/*
 * Acknowledgment: This code is from the Microsoft tutorial "Examine the generated database context class and registration"
 * @reference: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/adding-model?view=aspnetcore-8.0&tabs=visual-studio
 */
namespace EbbinghausFlashcardApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // no need to add user table, it is already included in IdentityDbContext<User>
        public DbSet<FlashcardSet> FlashcardSets { get; set; }
        public DbSet<Flashcard> Flashcards { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            DbSeeder.Seed(modelBuilder);
        }
    }
}
