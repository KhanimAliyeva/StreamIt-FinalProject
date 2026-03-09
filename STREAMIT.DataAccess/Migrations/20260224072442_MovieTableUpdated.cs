using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STREAMIT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MovieTableUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Movies_Imdb",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "Imdb",
                table: "Movies");

            migrationBuilder.AddColumn<decimal>(
                name: "AverageRating",
                table: "Movies",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "RatingCount",
                table: "Movies",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Movies");

            migrationBuilder.DropColumn(
                name: "RatingCount",
                table: "Movies");

            migrationBuilder.AddColumn<decimal>(
                name: "Imdb",
                table: "Movies",
                type: "decimal(3,1)",
                precision: 3,
                scale: 1,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Imdb",
                table: "Movies",
                column: "Imdb");
        }
    }
}
