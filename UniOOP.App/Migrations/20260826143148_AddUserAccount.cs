using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UniOOP.App.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAccount",
                schema: "public",
                columns: table => new
                {
                    user_account_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    user_type = table.Column<int>(type: "integer", nullable: false),
                    personnel_id = table.Column<long>(type: "bigint", nullable: true),
                    university_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_account", x => x.user_account_id);
                    table.CheckConstraint("CK_UserAccount_ExactlyOneOwner", "(\"personnel_id\" IS NOT NULL AND \"university_id\" IS NULL) OR (\"personnel_id\" IS NULL AND \"university_id\" IS NOT NULL)");
                    table.ForeignKey(
                        name: "fk_user_account_personnel_personnel_id",
                        column: x => x.personnel_id,
                        principalSchema: "public",
                        principalTable: "Personnel",
                        principalColumn: "personnel_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_account_university_university_id",
                        column: x => x.university_id,
                        principalSchema: "public",
                        principalTable: "University",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_account_personnel_id",
                schema: "public",
                table: "UserAccount",
                column: "personnel_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_account_university_id",
                schema: "public",
                table: "UserAccount",
                column: "university_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAccount",
                schema: "public");
        }
    }
}
