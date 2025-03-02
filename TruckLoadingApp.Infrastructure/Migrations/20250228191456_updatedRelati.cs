using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedRelati : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverPerformance_Drivers_DriverId1",
                table: "DriverPerformance");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverRoutePreference_Drivers_DriverId1",
                table: "DriverRoutePreference");

            migrationBuilder.DropForeignKey(
                name: "FK_DriverSchedule_Drivers_DriverId1",
                table: "DriverSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Drivers_AssignedDriverId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_Trucks_AssignedDriverId",
                table: "Trucks");

            migrationBuilder.DropIndex(
                name: "IX_DriverSchedule_DriverId1",
                table: "DriverSchedule");

            migrationBuilder.DropIndex(
                name: "IX_DriverRoutePreference_DriverId",
                table: "DriverRoutePreference");

            migrationBuilder.DropIndex(
                name: "IX_DriverRoutePreference_DriverId1",
                table: "DriverRoutePreference");

            migrationBuilder.DropIndex(
                name: "IX_DriverPerformance_DriverId1",
                table: "DriverPerformance");

            migrationBuilder.DropColumn(
                name: "DriverId1",
                table: "DriverSchedule");

            migrationBuilder.DropColumn(
                name: "DriverId1",
                table: "DriverRoutePreference");

            migrationBuilder.DropColumn(
                name: "DriverId1",
                table: "DriverPerformance");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRoutePreference_DriverId",
                table: "DriverRoutePreference",
                column: "DriverId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DriverRoutePreference_DriverId",
                table: "DriverRoutePreference");

            migrationBuilder.AddColumn<long>(
                name: "DriverId1",
                table: "DriverSchedule",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DriverId1",
                table: "DriverRoutePreference",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DriverId1",
                table: "DriverPerformance",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trucks_AssignedDriverId",
                table: "Trucks",
                column: "AssignedDriverId",
                unique: true,
                filter: "[AssignedDriverId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DriverSchedule_DriverId1",
                table: "DriverSchedule",
                column: "DriverId1");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRoutePreference_DriverId",
                table: "DriverRoutePreference",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRoutePreference_DriverId1",
                table: "DriverRoutePreference",
                column: "DriverId1",
                unique: true,
                filter: "[DriverId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPerformance_DriverId1",
                table: "DriverPerformance",
                column: "DriverId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverPerformance_Drivers_DriverId1",
                table: "DriverPerformance",
                column: "DriverId1",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverRoutePreference_Drivers_DriverId1",
                table: "DriverRoutePreference",
                column: "DriverId1",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverSchedule_Drivers_DriverId1",
                table: "DriverSchedule",
                column: "DriverId1",
                principalTable: "Drivers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Drivers_AssignedDriverId",
                table: "Trucks",
                column: "AssignedDriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
