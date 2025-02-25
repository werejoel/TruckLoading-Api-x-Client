using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixTruckAndDriverRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Trucks_TruckId",
                table: "Drivers");

            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Trucks_TruckId1",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_TruckId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_TruckId1",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "TruckId1",
                table: "Drivers");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_TruckId",
                table: "Drivers",
                column: "TruckId",
                unique: true,
                filter: "[TruckId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Trucks_TruckId",
                table: "Drivers",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Trucks_TruckId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_TruckId",
                table: "Drivers");

            migrationBuilder.AddColumn<int>(
                name: "TruckId1",
                table: "Drivers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_TruckId",
                table: "Drivers",
                column: "TruckId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_TruckId1",
                table: "Drivers",
                column: "TruckId1",
                unique: true,
                filter: "[TruckId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Trucks_TruckId",
                table: "Drivers",
                column: "TruckId",
                principalTable: "Trucks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Trucks_TruckId1",
                table: "Drivers",
                column: "TruckId1",
                principalTable: "Trucks",
                principalColumn: "Id");
        }
    }
}
