using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace P1_AP1_JorgeMoya.Migrations
{
    /// <inheritdoc />
    public partial class addTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TiposHuacales",
                columns: new[] { "TipoId", "Descripcion", "Existencia" },
                values: new object[,]
                {
                    { 4, "Azul", 0 },
                    { 5, "Amarillo", 0 },
                    { 6, "Naranja", 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TiposHuacales",
                keyColumn: "TipoId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TiposHuacales",
                keyColumn: "TipoId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TiposHuacales",
                keyColumn: "TipoId",
                keyValue: 6);
        }
    }
}
