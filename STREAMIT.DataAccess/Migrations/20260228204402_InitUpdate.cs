using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STREAMIT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePeople_People_PersonId",
                table: "MoviePeople");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePeople_People_PersonId",
                table: "MoviePeople",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MoviePeople_People_PersonId",
                table: "MoviePeople");

            migrationBuilder.AddForeignKey(
                name: "FK_MoviePeople_People_PersonId",
                table: "MoviePeople",
                column: "PersonId",
                principalTable: "People",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
