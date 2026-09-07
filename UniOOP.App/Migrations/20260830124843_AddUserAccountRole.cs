using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniOOP.App.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAccountRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "role",
                schema: "public",
                table: "UserAccount",
                type: "text",
                nullable: false,
                defaultValue: "User");

            migrationBuilder.Sql("""
                UPDATE "UserAccount"
                SET "role" = 'Admin'
                WHERE "personnel_id" = 14;
            """);
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "role",
                schema: "public",
                table: "UserAccount");
        }
    }
}
