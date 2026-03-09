using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STREAMIT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Added21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReviewTvShows_TvShowId",
                table: "ReviewTvShows");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ReviewTvShows",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTvShows_CreatedDate",
                table: "ReviewTvShows",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTvShows_Rating",
                table: "ReviewTvShows",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTvShows_TvShowId_UserId",
                table: "ReviewTvShows",
                columns: new[] { "TvShowId", "UserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ReviewTvShows_CreatedDate",
                table: "ReviewTvShows");

            migrationBuilder.DropIndex(
                name: "IX_ReviewTvShows_Rating",
                table: "ReviewTvShows");

            migrationBuilder.DropIndex(
                name: "IX_ReviewTvShows_TvShowId_UserId",
                table: "ReviewTvShows");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "ReviewTvShows",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTvShows_TvShowId",
                table: "ReviewTvShows",
                column: "TvShowId");
        }
    }
}
