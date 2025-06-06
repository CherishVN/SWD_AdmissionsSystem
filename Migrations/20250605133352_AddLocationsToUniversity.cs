using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AdmissionInfoSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationsToUniversity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "University",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ShortName = table.Column<string>(type: "text", nullable: false),
                    Introduction = table.Column<string>(type: "text", nullable: false),
                    OfficialWebsite = table.Column<string>(type: "text", nullable: false),
                    AdmissionWebsite = table.Column<string>(type: "text", nullable: false),
                    Ranking = table.Column<int>(type: "integer", nullable: true),
                    RankingCriteria = table.Column<string>(type: "text", nullable: false),
                    Locations = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_University", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionMethod",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Criteria = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UniversityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionMethod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionMethod_University_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "University",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionNew",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    PublishDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UniversityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionNew", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionNew_University_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "University",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Major",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    AdmissionScore = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UniversityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Major", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Major_University_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "University",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Program",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Tuition = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    TuitionUnit = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UniversityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Program", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Program_University_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "University",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scholarship",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    ValueType = table.Column<string>(type: "text", nullable: false),
                    Criteria = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UniversityId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scholarship", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scholarship_University_UniversityId",
                        column: x => x.UniversityId,
                        principalTable: "University",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdmissionCriteria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    MinimumScore = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    AdmissionMethodId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionCriteria_AdmissionMethod_AdmissionMethodId",
                        column: x => x.AdmissionMethodId,
                        principalTable: "AdmissionMethod",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionCriteria_AdmissionMethodId",
                table: "AdmissionCriteria",
                column: "AdmissionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionMethod_UniversityId",
                table: "AdmissionMethod",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionNew_UniversityId",
                table: "AdmissionNew",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Major_UniversityId",
                table: "Major",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Program_UniversityId",
                table: "Program",
                column: "UniversityId");

            migrationBuilder.CreateIndex(
                name: "IX_Scholarship_UniversityId",
                table: "Scholarship",
                column: "UniversityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdmissionCriteria");

            migrationBuilder.DropTable(
                name: "AdmissionNew");

            migrationBuilder.DropTable(
                name: "Major");

            migrationBuilder.DropTable(
                name: "Program");

            migrationBuilder.DropTable(
                name: "Scholarship");

            migrationBuilder.DropTable(
                name: "AdmissionMethod");

            migrationBuilder.DropTable(
                name: "University");
        }
    }
}
