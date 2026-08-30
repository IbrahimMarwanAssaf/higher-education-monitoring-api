using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniOOP.App.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyUserAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_user_account_university_university_id",
                schema: "public",
                table: "UserAccount");

            migrationBuilder.DropIndex(
                name: "ix_user_account_university_id",
                schema: "public",
                table: "UserAccount");

            migrationBuilder.DropCheckConstraint(
                name: "CK_UserAccount_ExactlyOneOwner",
                schema: "public",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "university_id",
                schema: "public",
                table: "UserAccount");

            migrationBuilder.DropColumn(
                name: "user_type",
                schema: "public",
                table: "UserAccount");

            migrationBuilder.AlterColumn<long>(
                name: "personnel_id",
                schema: "public",
                table: "UserAccount",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "personnel_id",
                schema: "public",
                table: "UserAccount",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "university_id",
                schema: "public",
                table: "UserAccount",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "user_type",
                schema: "public",
                table: "UserAccount",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "ix_user_account_university_id",
                schema: "public",
                table: "UserAccount",
                column: "university_id",
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_UserAccount_ExactlyOneOwner",
                schema: "public",
                table: "UserAccount",
                sql: "(\"personnel_id\" IS NOT NULL AND \"university_id\" IS NULL) OR (\"personnel_id\" IS NULL AND \"university_id\" IS NOT NULL)");

            migrationBuilder.AddForeignKey(
                name: "fk_user_account_university_university_id",
                schema: "public",
                table: "UserAccount",
                column: "university_id",
                principalSchema: "public",
                principalTable: "University",
                principalColumn: "university_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
