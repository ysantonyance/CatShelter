using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CatShelter.Data.Migrations
{
    /// <inheritdoc />
    public partial class DefinedCare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Care",
                columns: new[] { "Id", "CareName", "Description" },
                values: new object[,]
                {
                    { 1, "Food", "Cats will get food and water" },
                    { 2, "Medical Exam", "Each cat will have an opportunity to be medically examined" },
                    { 3, "Playtime", "You can come to our shelter to cheer the cats with play time" },
                    { 4, "Special Treatment", "Disabled or sick cats could get treatment based on their health problem" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Care",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Care",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Care",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Care",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
