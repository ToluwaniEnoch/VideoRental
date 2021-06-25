using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Api.Data.Migrations
{
    public partial class RefactorVideoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChildrenMovies");

            migrationBuilder.DropTable(
                name: "NewReleaseMovies");

            migrationBuilder.AddColumn<int>(
                name: "MaximumAge",
                table: "Videos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YearReleased",
                table: "Videos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaximumAge",
                table: "Videos");

            migrationBuilder.DropColumn(
                name: "YearReleased",
                table: "Videos");

            migrationBuilder.CreateTable(
                name: "ChildrenMovies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaximumAge = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChildrenMovies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewReleaseMovies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    YearReleased = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewReleaseMovies", x => x.Id);
                });
        }
    }
}
