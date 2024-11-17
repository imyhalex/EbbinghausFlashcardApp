using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EbbinghausFlashcardApp.Migrations
{
    /// <inheritdoc />
    public partial class MakeImagePathNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "FlashcardSets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Flashcards",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "FlashcardSets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "NextReviewDate" },
                values: new object[] { new DateTime(2024, 11, 17, 16, 46, 17, 937, DateTimeKind.Utc).AddTicks(4062), new DateTime(2024, 11, 17, 16, 46, 17, 937, DateTimeKind.Utc).AddTicks(4065) });

            migrationBuilder.UpdateData(
                table: "FlashcardSets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "NextReviewDate" },
                values: new object[] { new DateTime(2024, 11, 17, 16, 46, 17, 937, DateTimeKind.Utc).AddTicks(4068), new DateTime(2024, 11, 17, 16, 46, 17, 937, DateTimeKind.Utc).AddTicks(4069) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "FlashcardSets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImagePath",
                table: "Flashcards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "FlashcardSets",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedDate", "NextReviewDate" },
                values: new object[] { new DateTime(2024, 11, 9, 0, 43, 3, 56, DateTimeKind.Utc).AddTicks(3617), new DateTime(2024, 11, 9, 0, 43, 3, 56, DateTimeKind.Utc).AddTicks(3622) });

            migrationBuilder.UpdateData(
                table: "FlashcardSets",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedDate", "NextReviewDate" },
                values: new object[] { new DateTime(2024, 11, 8, 19, 43, 3, 56, DateTimeKind.Local).AddTicks(3628), new DateTime(2024, 11, 8, 19, 43, 3, 56, DateTimeKind.Local).AddTicks(3682) });
        }
    }
}
