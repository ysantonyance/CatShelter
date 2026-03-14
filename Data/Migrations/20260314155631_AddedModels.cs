using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatShelter.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Breed",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Breed", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Care",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CareName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Care", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Kg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Img = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BreedId = table.Column<int>(type: "int", nullable: false),
                    IsAdopted = table.Column<bool>(type: "bit", nullable: false),
                    IsHealthy = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cat_Breed_BreedId",
                        column: x => x.BreedId,
                        principalTable: "Breed",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Adoption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CatId = table.Column<int>(type: "int", nullable: false),
                    AdoptionDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adoption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adoption_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Adoption_Cat_CatId",
                        column: x => x.CatId,
                        principalTable: "Cat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatCare",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatId = table.Column<int>(type: "int", nullable: false),
                    CareId = table.Column<int>(type: "int", nullable: false),
                    IsSatisfied = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatCare", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatCare_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatCare_Care_CareId",
                        column: x => x.CareId,
                        principalTable: "Care",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatCare_Cat_CatId",
                        column: x => x.CatId,
                        principalTable: "Cat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adoption_CatId",
                table: "Adoption",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_Adoption_UserId",
                table: "Adoption",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_BreedId",
                table: "Cat",
                column: "BreedId");

            migrationBuilder.CreateIndex(
                name: "IX_CatCare_CareId",
                table: "CatCare",
                column: "CareId");

            migrationBuilder.CreateIndex(
                name: "IX_CatCare_CatId",
                table: "CatCare",
                column: "CatId");

            migrationBuilder.CreateIndex(
                name: "IX_CatCare_UserId",
                table: "CatCare",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adoption");

            migrationBuilder.DropTable(
                name: "CatCare");

            migrationBuilder.DropTable(
                name: "Care");

            migrationBuilder.DropTable(
                name: "Cat");

            migrationBuilder.DropTable(
                name: "Breed");
        }
    }
}
