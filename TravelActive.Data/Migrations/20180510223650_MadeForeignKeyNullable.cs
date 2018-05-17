using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TravelActive.Data.Migrations
{
    public partial class MadeForeignKeyNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusStops_Cities_CityId",
                table: "BusStops");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "BusStops",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_BusStops_Cities_CityId",
                table: "BusStops",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusStops_Cities_CityId",
                table: "BusStops");

            migrationBuilder.AlterColumn<int>(
                name: "CityId",
                table: "BusStops",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusStops_Cities_CityId",
                table: "BusStops",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
