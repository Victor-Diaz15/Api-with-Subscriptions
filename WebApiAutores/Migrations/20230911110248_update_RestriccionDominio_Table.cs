using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApiAutores.Migrations
{
    public partial class update_RestriccionDominio_Table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionesDominio_LlavesApi_LlaveApiId",
                table: "RestriccionesDominio");

            migrationBuilder.DropColumn(
                name: "LlaveId",
                table: "RestriccionesDominio");

            migrationBuilder.AlterColumn<int>(
                name: "LlaveApiId",
                table: "RestriccionesDominio",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionesDominio_LlavesApi_LlaveApiId",
                table: "RestriccionesDominio",
                column: "LlaveApiId",
                principalTable: "LlavesApi",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RestriccionesDominio_LlavesApi_LlaveApiId",
                table: "RestriccionesDominio");

            migrationBuilder.AlterColumn<int>(
                name: "LlaveApiId",
                table: "RestriccionesDominio",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "LlaveId",
                table: "RestriccionesDominio",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_RestriccionesDominio_LlavesApi_LlaveApiId",
                table: "RestriccionesDominio",
                column: "LlaveApiId",
                principalTable: "LlavesApi",
                principalColumn: "Id");
        }
    }
}
