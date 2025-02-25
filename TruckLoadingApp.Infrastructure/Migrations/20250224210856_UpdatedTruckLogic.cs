using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTruckLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TruckLocationHistories_Bookings_BookingId",
                table: "TruckLocationHistories");

            migrationBuilder.DropIndex(
                name: "IX_TruckLocationHistories_BookingId",
                table: "TruckLocationHistories");

            migrationBuilder.DropColumn(
                name: "AvailableCapacityVolume",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "DriverContactInformation",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "DriverName",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "InsuranceInformation",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "PreferredRoute",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "TruckLocationHistories");

            migrationBuilder.AlterColumn<int>(
                name: "TruckId",
                table: "Drivers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TruckId1",
                table: "Drivers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_TruckId1",
                table: "Drivers",
                column: "TruckId1",
                unique: true,
                filter: "[TruckId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_Trucks_TruckId1",
                table: "Drivers",
                column: "TruckId1",
                principalTable: "Trucks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_Trucks_TruckId1",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_TruckId1",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "TruckId1",
                table: "Drivers");

            migrationBuilder.AddColumn<decimal>(
                name: "AvailableCapacityVolume",
                table: "Trucks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "DriverContactInformation",
                table: "Trucks",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DriverName",
                table: "Trucks",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InsuranceInformation",
                table: "Trucks",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredRoute",
                table: "Trucks",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BookingId",
                table: "TruckLocationHistories",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TruckId",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TruckLocationHistories_BookingId",
                table: "TruckLocationHistories",
                column: "BookingId");

            migrationBuilder.AddForeignKey(
                name: "FK_TruckLocationHistories_Bookings_BookingId",
                table: "TruckLocationHistories",
                column: "BookingId",
                principalTable: "Bookings",
                principalColumn: "Id");
        }
    }
}
