using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace P1_AP1_JorgeMoya.Migrations
{
    /// <inheritdoc />
    public partial class TableEntradas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Registros",
                table: "Registros");

            migrationBuilder.RenameTable(
                name: "Registros",
                newName: "EntradasHuacales");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntradasHuacales",
                table: "EntradasHuacales",
                column: "IdEntrada");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EntradasHuacales",
                table: "EntradasHuacales");

            migrationBuilder.RenameTable(
                name: "EntradasHuacales",
                newName: "Registros");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Registros",
                table: "Registros",
                column: "IdEntrada");
        }
    }
}
