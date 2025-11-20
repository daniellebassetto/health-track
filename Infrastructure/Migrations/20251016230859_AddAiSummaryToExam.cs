using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddAiSummaryToExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "resumo_ia",
                table: "exames",
                type: "TEXT",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "resumo_ia",
                table: "exames");
        }
    }
}
