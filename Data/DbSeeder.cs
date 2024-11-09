using EbbinghausFlashcardApp.Models;
using Microsoft.EntityFrameworkCore;

/*
 * Acknowledgment: This code is from the Microsoft tutorial "Part 5, work with a database in an ASP.NET Core MVC app"
 * @reference : https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/working-with-sql?view=aspnetcore-8.0&tabs=visual-studio
 */
namespace EbbinghausFlashcardApp.Data
{
    public class DbSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FlashcardSet>().HasData(
                new FlashcardSet
                {
                    Id = 1,
                    Name = "GRE verbal vocabs",
                    Description = "Essential Spanish words for beginners",
                    CreatedDate = DateTime.UtcNow,
                    ReviewInterval = -1,
                    NextReviewDate = DateTime.UtcNow,
                    ImagePath = "/images/gre.jpg",
                    UserId = "b0dbe1be-79c1-4ea8-aeb8-56ca5314922e"
                },
                new FlashcardSet
                {
                    Id = 2,
                    Name = "Linear Algebra",
                    Description = "Linear Algebra Crash Course",
                    CreatedDate = DateTime.UtcNow,
                    ReviewInterval = -1,
                    NextReviewDate = DateTime.UtcNow,
                    ImagePath = "/images/linear_algebra.jpg",
                    UserId = "b0dbe1be-79c1-4ea8-aeb8-56ca5314922e"
                }
            );

            modelBuilder.Entity<Flashcard>().HasData(
                new Flashcard
                {
                    Id = 1,
                    Term = "abate",
                    Definition = "to reduce in amount, degree, or severity",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    Id = 2,
                    Term = "aberrant",
                    Definition = "deviating from the norm",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    Id = 3,
                    Term = "abeyance",
                    Definition = "temporary suppression or suspension",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    Id = 4,
                    Term = "abjure",
                    Definition = "to reject; abandon formally",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    Id = 5,
                    Term = "abrogate",
                    Definition = "to abolish, usually by authority",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    Id = 6,
                    Term = "abscond",
                    Definition = "to leave hurriedly and secretly, typically to avoid detection or arrest",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    Id = 7,
                    Term = "abstemious",
                    Definition = "sparing in eating and drinking; temperate",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    Id = 8,
                    Term = "admonish",
                    Definition = "to caution or advise against something; to scold mildly; to remind of a duty",
                    IsFamiliar = false,
                    ImagePath = "/images/gre.jpg",
                    FlashcardSetId = 1
                },
                new Flashcard
                {
                    // give me some lienar algebra terms
                    Id = 9,
                    Term = "Matrix",
                    Definition = "A matrix is a collection of numbers arranged into a fixed number of rows and columns.",
                    IsFamiliar = false,
                    ImagePath = "/images/linear_algebra.jpg",
                    FlashcardSetId = 2
                },
                new Flashcard
                {
                    Id = 10,
                    Term = "Determinant",
                    Definition = "A determinant is a scalar value derived from a square matrix.",
                    IsFamiliar = false,
                    ImagePath = "/images/linear_algebra.jpg",
                    FlashcardSetId = 2
                },
                new Flashcard
                {
                    Id = 11,
                    Term = "Eigenvalue",
                    Definition = "An eigenvalue is a scalar that is a special set of scalars associated with a linear system of equations.",
                    IsFamiliar = false,
                    ImagePath = "/images/linear_algebra.jpg",
                    FlashcardSetId = 2
                },
                new Flashcard
                {
                    Id = 12,
                    Term = "Eigenvector",
                    Definition = "An eigenvector is a nonzero vector that stays in the same direction after a linear transformation.",
                    IsFamiliar = false,
                    ImagePath = "/images/linear_algebra.jpg",
                    FlashcardSetId = 2
                },
                new Flashcard
                {
                    Id = 13,
                    Term = "Transpose",
                    Definition = "The transpose of a matrix is a new matrix whose rows are the columns of the original.",
                    IsFamiliar = false,
                    ImagePath = "/images/linear_algebra.jpg",
                    FlashcardSetId = 2
                }
            );
        }
    }
}
