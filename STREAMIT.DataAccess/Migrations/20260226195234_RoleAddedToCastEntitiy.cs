using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace STREAMIT.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RoleAddedToCastEntitiy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "People",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "People");
        }
    }
}
