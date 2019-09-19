using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GraphLinqQL.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Films",
                columns: table => new
                {
                    EpisodeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Films", x => x.EpisodeId);
                });

            migrationBuilder.CreateTable(
                name: "FilmCharacter",
                columns: table => new
                {
                    EpisodeId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmCharacter", x => new { x.EpisodeId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_FilmCharacter_Films_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Films",
                        principalColumn: "EpisodeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmCharacter_Characters_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Characters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Characters",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Luke Skywalker" },
                    { 2, "C-3PO" },
                    { 3, "R2-D2" }
                });

            migrationBuilder.InsertData(
                table: "Films",
                columns: new[] { "EpisodeId", "Title" },
                values: new object[] { 1, "A New Hope" });

            migrationBuilder.InsertData(
                table: "FilmCharacter",
                columns: new[] { "EpisodeId", "PersonId" },
                values: new object[] { 1, 1 });

            migrationBuilder.InsertData(
                table: "FilmCharacter",
                columns: new[] { "EpisodeId", "PersonId" },
                values: new object[] { 1, 2 });

            migrationBuilder.InsertData(
                table: "FilmCharacter",
                columns: new[] { "EpisodeId", "PersonId" },
                values: new object[] { 1, 3 });

            migrationBuilder.CreateIndex(
                name: "IX_FilmCharacter_PersonId",
                table: "FilmCharacter",
                column: "PersonId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmCharacter");

            migrationBuilder.DropTable(
                name: "Films");

            migrationBuilder.DropTable(
                name: "Characters");
        }
    }
}
