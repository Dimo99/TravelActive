using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelActive.Data.Migrations
{
    public partial class ChangesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusesToBusStops_BusStops_BusId",
                table: "BusesToBusStops");

            migrationBuilder.DropForeignKey(
                name: "FK_BusesToBusStops_Busses_BusStopId",
                table: "BusesToBusStops");

            migrationBuilder.AddForeignKey(
                name: "FK_BusesToBusStops_Busses_BusId",
                table: "BusesToBusStops",
                column: "BusId",
                principalTable: "Busses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusesToBusStops_BusStops_BusStopId",
                table: "BusesToBusStops",
                column: "BusStopId",
                principalTable: "BusStops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusesToBusStops_Busses_BusId",
                table: "BusesToBusStops");

            migrationBuilder.DropForeignKey(
                name: "FK_BusesToBusStops_BusStops_BusStopId",
                table: "BusesToBusStops");

            migrationBuilder.AddForeignKey(
                name: "FK_BusesToBusStops_BusStops_BusId",
                table: "BusesToBusStops",
                column: "BusId",
                principalTable: "BusStops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BusesToBusStops_Busses_BusStopId",
                table: "BusesToBusStops",
                column: "BusStopId",
                principalTable: "Busses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
