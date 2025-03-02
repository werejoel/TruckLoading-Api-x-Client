using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameToDriverDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PaymentDate",
                table: "PayrollEntry",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "DriverDocument",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TemperatureReadings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoadId = table.Column<long>(type: "bigint", nullable: false),
                    Temperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeviceId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsWithinRange = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemperatureReadings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemperatureReadings_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TruckHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TruckId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviousStatus = table.Column<int>(type: "int", nullable: true),
                    NewStatus = table.Column<int>(type: "int", nullable: true),
                    PreviousDriverId = table.Column<long>(type: "bigint", nullable: true),
                    NewDriverId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TruckHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TruckHistory_Trucks_TruckId",
                        column: x => x.TruckId,
                        principalTable: "Trucks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemperatureReadings_LoadId",
                table: "TemperatureReadings",
                column: "LoadId");

            migrationBuilder.CreateIndex(
                name: "IX_TruckHistory_TruckId",
                table: "TruckHistory",
                column: "TruckId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemperatureReadings");

            migrationBuilder.DropTable(
                name: "TruckHistory");

            migrationBuilder.DropColumn(
                name: "PaymentDate",
                table: "PayrollEntry");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "DriverDocument");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "AspNetUsers");
        }
    }
}
