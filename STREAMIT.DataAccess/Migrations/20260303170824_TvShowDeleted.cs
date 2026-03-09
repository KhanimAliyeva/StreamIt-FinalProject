using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STREAMIT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class TvShowDeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Seasons_TVShows_TVShowId",
                table: "Seasons");

            migrationBuilder.DropForeignKey(
                name: "FK_TvShowGenres_TVShows_TvShowId",
                table: "TvShowGenres");

            migrationBuilder.DropTable(
                name: "ReviewTvShows");

            migrationBuilder.DropTable(
                name: "TvShowPeople");

            migrationBuilder.DropTable(
                name: "TvShowStatistics");

            migrationBuilder.DropTable(
                name: "TvShowTags");

            migrationBuilder.DropTable(
                name: "UserTvShows");

            migrationBuilder.DropTable(
                name: "TVShows");

            migrationBuilder.DropIndex(
                name: "IX_TvShowGenres_TvShowId_GenreId",
                table: "TvShowGenres");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TVShows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageId = table.Column<int>(type: "int", nullable: false),
                    MembershipId = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Imdb = table.Column<decimal>(type: "decimal(3,1)", precision: 3, scale: 1, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    PosterUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RatingCount = table.Column<int>(type: "int", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    YoutubeUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TVShows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TVShows_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TVShows_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewTvShows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TvShowId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(2,1)", precision: 2, scale: 1, nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTvShows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReviewTvShows_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewTvShows_TVShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TVShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TvShowPeople",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    TvShowId = table.Column<int>(type: "int", nullable: false),
                    CastOrder = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowPeople", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TvShowPeople_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TvShowPeople_TVShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TVShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TvShowStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TvShowId = table.Column<int>(type: "int", nullable: false),
                    LikeCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ViewCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TvShowStatistics_TVShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TVShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TvShowTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TagId = table.Column<int>(type: "int", nullable: false),
                    TvShowId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TvShowTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TvShowTags_TVShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TVShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TvShowTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTvShows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TvShowId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTvShows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTvShows_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTvShows_TVShows_TvShowId",
                        column: x => x.TvShowId,
                        principalTable: "TVShows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TvShowGenres_TvShowId_GenreId",
                table: "TvShowGenres",
                columns: new[] { "TvShowId", "GenreId" },
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_ReviewTvShows_UserId_TvShowId",
                table: "ReviewTvShows",
                columns: new[] { "UserId", "TvShowId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TvShowPeople_PersonId",
                table: "TvShowPeople",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowPeople_TvShowId_CastOrder",
                table: "TvShowPeople",
                columns: new[] { "TvShowId", "CastOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_TvShowPeople_TvShowId_PersonId",
                table: "TvShowPeople",
                columns: new[] { "TvShowId", "PersonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TVShows_Imdb",
                table: "TVShows",
                column: "Imdb");

            migrationBuilder.CreateIndex(
                name: "IX_TVShows_LanguageId",
                table: "TVShows",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TVShows_MembershipId",
                table: "TVShows",
                column: "MembershipId");

            migrationBuilder.CreateIndex(
                name: "IX_TVShows_ReleaseDate",
                table: "TVShows",
                column: "ReleaseDate");

            migrationBuilder.CreateIndex(
                name: "IX_TVShows_Title",
                table: "TVShows",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowStatistics_TvShowId",
                table: "TvShowStatistics",
                column: "TvShowId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TvShowTags_TagId",
                table: "TvShowTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TvShowTags_TvShowId_TagId",
                table: "TvShowTags",
                columns: new[] { "TvShowId", "TagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTvShows_TvShowId",
                table: "UserTvShows",
                column: "TvShowId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTvShows_UserId_TvShowId",
                table: "UserTvShows",
                columns: new[] { "UserId", "TvShowId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Seasons_TVShows_TVShowId",
                table: "Seasons",
                column: "TVShowId",
                principalTable: "TVShows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TvShowGenres_TVShows_TvShowId",
                table: "TvShowGenres",
                column: "TvShowId",
                principalTable: "TVShows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
