using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TravelActive.Data.Migrations
{
    public partial class ChangedKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BusesToBusStops",
                table: "BusesToBusStops");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BusesToBusStops",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusesToBusStops",
                table: "BusesToBusStops",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_BusesToBusStops_BusId",
                table: "BusesToBusStops",
                column: "BusId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BusesToBusStops",
                table: "BusesToBusStops");

            migrationBuilder.DropIndex(
                name: "IX_BusesToBusStops_BusId",
                table: "BusesToBusStops");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BusesToBusStops");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BusesToBusStops",
                table: "BusesToBusStops",
                columns: new[] { "BusId", "BusStopId" });
        }
    }
}
