using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    public partial class Restricciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RestriccionesDominio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    Dominio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LlaveApiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesDominio", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesDominio_LlavesApi_LlaveApiId",
                        column: x => x.LlaveApiId,
                        principalTable: "LlavesApi",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RestriccionesIP",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LlaveId = table.Column<int>(type: "int", nullable: false),
                    IP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LlaveApiId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestriccionesIP", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestriccionesIP_LlavesApi_LlaveApiId",
                        column: x => x.LlaveApiId,
                        principalTable: "LlavesApi",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesDominio_LlaveApiId",
                table: "RestriccionesDominio",
                column: "LlaveApiId");

            migrationBuilder.CreateIndex(
                name: "IX_RestriccionesIP_LlaveApiId",
                table: "RestriccionesIP",
                column: "LlaveApiId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestriccionesDominio");

            migrationBuilder.DropTable(
                name: "RestriccionesIP");
        }
    }
}
