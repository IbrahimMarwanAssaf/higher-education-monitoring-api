using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UniOOP.App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "Personnel",
                schema: "public",
                columns: table => new
                {
                    personnel_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ssn = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    f_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    l_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    date_of_birth = table.Column<DateOnly>(type: "date", nullable: false),
                    email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personnel", x => x.personnel_id);
                });

            migrationBuilder.CreateTable(
                name: "University",
                schema: "public",
                columns: table => new
                {
                    university_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    university_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_University", x => x.university_id);
                });

            migrationBuilder.CreateTable(
                name: "GovernmentOfficer",
                schema: "public",
                columns: table => new
                {
                    personnel_id = table.Column<long>(type: "bigint", nullable: false),
                    officer_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GovernmentOfficer", x => x.personnel_id);
                    table.ForeignKey(
                        name: "fk_government_officer_personnel_personnel_id",
                        column: x => x.personnel_id,
                        principalSchema: "public",
                        principalTable: "Personnel",
                        principalColumn: "personnel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Student",
                schema: "public",
                columns: table => new
                {
                    personnel_id = table.Column<long>(type: "bigint", nullable: false),
                    student_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    major = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    gpa = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    university_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Student", x => x.personnel_id);
                    table.CheckConstraint("CK_Student_GPA", "\"gpa\" >= 0.00 AND \"gpa\" <= 4.00");
                    table.ForeignKey(
                        name: "FK_Student_University",
                        column: x => x.university_id,
                        principalSchema: "public",
                        principalTable: "University",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_student_personnel_personnel_id",
                        column: x => x.personnel_id,
                        principalSchema: "public",
                        principalTable: "Personnel",
                        principalColumn: "personnel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
                schema: "public",
                columns: table => new
                {
                    personnel_id = table.Column<long>(type: "bigint", nullable: false),
                    teacher_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    department = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    salary = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    ministry_degree_id = table.Column<int>(type: "integer", nullable: true),
                    university_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teacher", x => x.personnel_id);
                    table.CheckConstraint("CK_Teachert_Salary", "\"salary\" >= 0 ");
                    table.ForeignKey(
                        name: "FK_Teacher_University",
                        column: x => x.university_id,
                        principalSchema: "public",
                        principalTable: "University",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_teacher_personnel_personnel_id",
                        column: x => x.personnel_id,
                        principalSchema: "public",
                        principalTable: "Personnel",
                        principalColumn: "personnel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Course",
                schema: "public",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    course_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    credits = table.Column<int>(type: "integer", nullable: false),
                    university_id = table.Column<int>(type: "integer", nullable: false),
                    teacher_personnel_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Course", x => x.course_id);
                    table.CheckConstraint("CK_Course_Credits", "credits BETWEEN 1 AND 6");
                    table.ForeignKey(
                        name: "FK_Course_Teacher",
                        column: x => x.teacher_personnel_id,
                        principalSchema: "public",
                        principalTable: "Teacher",
                        principalColumn: "personnel_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Course_University",
                        column: x => x.university_id,
                        principalSchema: "public",
                        principalTable: "University",
                        principalColumn: "university_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentCourse",
                schema: "public",
                columns: table => new
                {
                    course_id = table.Column<int>(type: "integer", nullable: false),
                    student_personnel_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourse", x => new { x.student_personnel_id, x.course_id });
                    table.ForeignKey(
                        name: "FK_StudentCourse_Course",
                        column: x => x.course_id,
                        principalSchema: "public",
                        principalTable: "Course",
                        principalColumn: "course_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourse_Student",
                        column: x => x.student_personnel_id,
                        principalSchema: "public",
                        principalTable: "Student",
                        principalColumn: "personnel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Course_TeacherPersonnelID",
                schema: "public",
                table: "Course",
                column: "teacher_personnel_id");

            migrationBuilder.CreateIndex(
                name: "IX_Course_UniversityID",
                schema: "public",
                table: "Course",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "UX_Course_UniversityID_CourseName",
                schema: "public",
                table: "Course",
                columns: new[] { "university_id", "course_name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_GovernmentOfficer_OfficerID",
                schema: "public",
                table: "GovernmentOfficer",
                column: "officer_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Personnel_Email",
                schema: "public",
                table: "Personnel",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_Personnel_SSN",
                schema: "public",
                table: "Personnel",
                column: "ssn",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Student_UniversityID",
                schema: "public",
                table: "Student",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "UX_Student_StudentID",
                schema: "public",
                table: "Student",
                column: "student_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourse_CourseID",
                schema: "public",
                table: "StudentCourse",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_UniversityID",
                schema: "public",
                table: "Teacher",
                column: "university_id");

            migrationBuilder.CreateIndex(
                name: "UX_Teacher_TeacherID",
                schema: "public",
                table: "Teacher",
                column: "teacher_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_University_UniversityName",
                schema: "public",
                table: "University",
                column: "university_name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GovernmentOfficer",
                schema: "public");

            migrationBuilder.DropTable(
                name: "StudentCourse",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Course",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Student",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Teacher",
                schema: "public");

            migrationBuilder.DropTable(
                name: "University",
                schema: "public");

            migrationBuilder.DropTable(
                name: "Personnel",
                schema: "public");
        }
    }
}
