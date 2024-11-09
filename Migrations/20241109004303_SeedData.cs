using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EbbinghausFlashcardApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FlashcardSets",
                columns: new[] { "Id", "CreatedDate", "Description", "ImagePath", "Name", "NextReviewDate", "ReviewInterval", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 11, 9, 0, 43, 3, 56, DateTimeKind.Utc).AddTicks(3617), "Essential Spanish words for beginners", "/images/gre.jpg", "GRE verbal vocabs", new DateTime(2024, 11, 9, 0, 43, 3, 56, DateTimeKind.Utc).AddTicks(3622), -1, "b0dbe1be-79c1-4ea8-aeb8-56ca5314922e" },
                    { 2, new DateTime(2024, 11, 8, 19, 43, 3, 56, DateTimeKind.Local).AddTicks(3628), "Linear Algebra Crash Course", "/images/linear_algebra.jpg", "Linear Algebra", new DateTime(2024, 11, 8, 19, 43, 3, 56, DateTimeKind.Local).AddTicks(3682), -1, "b0dbe1be-79c1-4ea8-aeb8-56ca5314922e" }
                });

            migrationBuilder.InsertData(
                table: "Flashcards",
                columns: new[] { "Id", "Definition", "FlashcardSetId", "ImagePath", "IsFamiliar", "Term" },
                values: new object[,]
                {
                    { 1, "to reduce in amount, degree, or severity", 1, "/images/gre.jpg", false, "abate" },
                    { 2, "deviating from the norm", 1, "/images/gre.jpg", false, "aberrant" },
                    { 3, "temporary suppression or suspension", 1, "/images/gre.jpg", false, "abeyance" },
                    { 4, "to reject; abandon formally", 1, "/images/gre.jpg", false, "abjure" },
                    { 5, "to abolish, usually by authority", 1, "/images/gre.jpg", false, "abrogate" },
                    { 6, "to leave hurriedly and secretly, typically to avoid detection or arrest", 1, "/images/gre.jpg", false, "abscond" },
                    { 7, "sparing in eating and drinking; temperate", 1, "/images/gre.jpg", false, "abstemious" },
                    { 8, "to caution or advise against something; to scold mildly; to remind of a duty", 1, "/images/gre.jpg", false, "admonish" },
                    { 9, "A matrix is a collection of numbers arranged into a fixed number of rows and columns.", 2, "/images/linear_algebra.jpg", false, "Matrix" },
                    { 10, "A determinant is a scalar value derived from a square matrix.", 2, "/images/linear_algebra.jpg", false, "Determinant" },
                    { 11, "An eigenvalue is a scalar that is a special set of scalars associated with a linear system of equations.", 2, "/images/linear_algebra.jpg", false, "Eigenvalue" },
                    { 12, "An eigenvector is a nonzero vector that stays in the same direction after a linear transformation.", 2, "/images/linear_algebra.jpg", false, "Eigenvector" },
                    { 13, "The transpose of a matrix is a new matrix whose rows are the columns of the original.", 2, "/images/linear_algebra.jpg", false, "Transpose" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Flashcards",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "FlashcardSets",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FlashcardSets",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
