using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace LF10.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FilterCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterCategories", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FilterPhrases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    CursePhrase = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    FilterPhrase = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterPhrases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterPhrases_FilterCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FilterCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FilterWords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Language = table.Column<string>(type: "varchar(5)", maxLength: 5, nullable: false),
                    CurseWord = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    FilterWord = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterWords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilterWords_FilterCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "FilterCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FilterCategories_Name",
                table: "FilterCategories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilterPhrases_CategoryId",
                table: "FilterPhrases",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterPhrases_CursePhrase",
                table: "FilterPhrases",
                column: "CursePhrase",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilterWords_CategoryId",
                table: "FilterWords",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FilterWords_CurseWord",
                table: "FilterWords",
                column: "CurseWord",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilterPhrases");

            migrationBuilder.DropTable(
                name: "FilterWords");

            migrationBuilder.DropTable(
                name: "FilterCategories");
        }
    }
}
