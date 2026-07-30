using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UniOOP.App.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEnrollmentDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourse_Course",
                schema: "public",
                table: "StudentCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourse_Student",
                schema: "public",
                table: "StudentCourse");

            migrationBuilder.AlterColumn<short>(
                name: "credits",
                schema: "public",
                table: "Course",
                type: "smallint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourse_Course",
                schema: "public",
                table: "StudentCourse",
                column: "course_id",
                principalSchema: "public",
                principalTable: "Course",
                principalColumn: "course_id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourse_Student",
                schema: "public",
                table: "StudentCourse",
                column: "student_personnel_id",
                principalSchema: "public",
                principalTable: "Student",
                principalColumn: "personnel_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourse_Course",
                schema: "public",
                table: "StudentCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentCourse_Student",
                schema: "public",
                table: "StudentCourse");

            migrationBuilder.AlterColumn<int>(
                name: "credits",
                schema: "public",
                table: "Course",
                type: "integer",
                nullable: false,
                oldClrType: typeof(short),
                oldType: "smallint");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourse_Course",
                schema: "public",
                table: "StudentCourse",
                column: "course_id",
                principalSchema: "public",
                principalTable: "Course",
                principalColumn: "course_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCourse_Student",
                schema: "public",
                table: "StudentCourse",
                column: "student_personnel_id",
                principalSchema: "public",
                principalTable: "Student",
                principalColumn: "personnel_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
