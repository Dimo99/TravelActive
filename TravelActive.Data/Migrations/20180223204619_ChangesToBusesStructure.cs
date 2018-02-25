using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TravelActive.Data.Migrations
{
    public partial class ChangesToBusesStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusesToBusStops");

            migrationBuilder.DropTable(
                name: "Time");

            migrationBuilder.DropTable(
                name: "BusBusStopTimes");

            migrationBuilder.CreateTable(
                name: "DepartureTimes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusId = table.Column<int>(nullable: false),
                    Departuretime = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartureTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DepartureTimes_Busses_BusId",
                        column: x => x.BusId,
                        principalTable: "Busses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StopsAccessibility",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusId = table.Column<int>(nullable: false),
                    DestStopId = table.Column<int>(nullable: false),
                    InitialStopId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopsAccessibility", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StopsAccessibility_Busses_BusId",
                        column: x => x.BusId,
                        principalTable: "Busses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_StopsAccessibility_BusStops_DestStopId",
                        column: x => x.DestStopId,
                        principalTable: "BusStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_StopsAccessibility_BusStops_InitialStopId",
                        column: x => x.InitialStopId,
                        principalTable: "BusStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "StopsOrdered",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusId = table.Column<int>(nullable: false),
                    BusStopId = table.Column<int>(nullable: false),
                    Delay = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StopsOrdered", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StopsOrdered_Busses_BusId",
                        column: x => x.BusId,
                        principalTable: "Busses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StopsOrdered_BusStops_BusStopId",
                        column: x => x.BusStopId,
                        principalTable: "BusStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartureTimes_BusId",
                table: "DepartureTimes",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_StopsAccessibility_BusId",
                table: "StopsAccessibility",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_StopsAccessibility_DestStopId",
                table: "StopsAccessibility",
                column: "DestStopId");

            migrationBuilder.CreateIndex(
                name: "IX_StopsAccessibility_InitialStopId",
                table: "StopsAccessibility",
                column: "InitialStopId");

            migrationBuilder.CreateIndex(
                name: "IX_StopsOrdered_BusId",
                table: "StopsOrdered",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_StopsOrdered_BusStopId",
                table: "StopsOrdered",
                column: "BusStopId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartureTimes");

            migrationBuilder.DropTable(
                name: "StopsAccessibility");

            migrationBuilder.DropTable(
                name: "StopsOrdered");

            migrationBuilder.CreateTable(
                name: "BusBusStopTimes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusId = table.Column<int>(nullable: true),
                    StopName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusBusStopTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusBusStopTimes_Busses_BusId",
                        column: x => x.BusId,
                        principalTable: "Busses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BusesToBusStops",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusId = table.Column<int>(nullable: false),
                    BusStopId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusesToBusStops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusesToBusStops_Busses_BusId",
                        column: x => x.BusId,
                        principalTable: "Busses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusesToBusStops_BusStops_BusStopId",
                        column: x => x.BusStopId,
                        principalTable: "BusStops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Time",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BusBusStopTimeId = table.Column<int>(nullable: false),
                    TimeOfDay = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Time", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Time_BusBusStopTimes_BusBusStopTimeId",
                        column: x => x.BusBusStopTimeId,
                        principalTable: "BusBusStopTimes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusBusStopTimes_BusId",
                table: "BusBusStopTimes",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusesToBusStops_BusId",
                table: "BusesToBusStops",
                column: "BusId");

            migrationBuilder.CreateIndex(
                name: "IX_BusesToBusStops_BusStopId",
                table: "BusesToBusStops",
                column: "BusStopId");

            migrationBuilder.CreateIndex(
                name: "IX_Time_BusBusStopTimeId",
                table: "Time",
                column: "BusBusStopTimeId");
        }
    }
}
