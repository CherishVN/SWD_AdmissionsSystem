using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AdmissionInfoSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddProgramIdToMajor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgramId",
                table: "Major",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Major_ProgramId",
                table: "Major",
                column: "ProgramId");

            migrationBuilder.AddForeignKey(
                name: "FK_Major_Program_ProgramId",
                table: "Major",
                column: "ProgramId",
                principalTable: "Program",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Major_Program_ProgramId",
                table: "Major");

            migrationBuilder.DropIndex(
                name: "IX_Major_ProgramId",
                table: "Major");

            migrationBuilder.DropColumn(
                name: "ProgramId",
                table: "Major");
        }
    }
}
