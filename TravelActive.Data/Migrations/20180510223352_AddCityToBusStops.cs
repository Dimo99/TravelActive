using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TravelActive.Data.Migrations
{
    public partial class AddCityToBusStops : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "BusStops",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BusStops_CityId",
                table: "BusStops",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_BusStops_Cities_CityId",
                table: "BusStops",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusStops_Cities_CityId",
                table: "BusStops");

            migrationBuilder.DropIndex(
                name: "IX_BusStops_CityId",
                table: "BusStops");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "BusStops");
        }
    }
}
