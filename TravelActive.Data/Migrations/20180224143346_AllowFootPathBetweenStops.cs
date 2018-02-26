using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelActive.Data.Migrations
{
    public partial class AllowFootPathBetweenStops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StopsAccessibility_Busses_BusId",
                table: "StopsAccessibility");

            migrationBuilder.AlterColumn<int>(
                name: "BusId",
                table: "StopsAccessibility",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_StopsAccessibility_Busses_BusId",
                table: "StopsAccessibility",
                column: "BusId",
                principalTable: "Busses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StopsAccessibility_Busses_BusId",
                table: "StopsAccessibility");

            migrationBuilder.AlterColumn<int>(
                name: "BusId",
                table: "StopsAccessibility",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StopsAccessibility_Busses_BusId",
                table: "StopsAccessibility",
                column: "BusId",
                principalTable: "Busses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
