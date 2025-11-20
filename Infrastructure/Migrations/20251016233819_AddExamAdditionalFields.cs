using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthTrack.Migrations
{
    /// <inheritdoc />
    public partial class AddExamAdditionalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "crm_medico_solicitante",
                table: "exames",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "laboratorio",
                table: "exames",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "nome_convenio",
                table: "exames",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "nome_medico_solicitante",
                table: "exames",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "tem_convenio",
                table: "exames",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "crm_medico_solicitante",
                table: "exames");

            migrationBuilder.DropColumn(
                name: "laboratorio",
                table: "exames");

            migrationBuilder.DropColumn(
                name: "nome_convenio",
                table: "exames");

            migrationBuilder.DropColumn(
                name: "nome_medico_solicitante",
                table: "exames");

            migrationBuilder.DropColumn(
                name: "tem_convenio",
                table: "exames");
        }
    }
}
