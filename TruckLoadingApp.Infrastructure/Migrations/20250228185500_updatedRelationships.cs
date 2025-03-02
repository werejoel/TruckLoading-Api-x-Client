using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverSchedule_Loads_LoadId",
                table: "DriverSchedule");

            migrationBuilder.DropIndex(
                name: "IX_DriverRoutePreference_DriverId",
                table: "DriverRoutePreference");

            migrationBuilder.AlterColumn<decimal>(
                name: "FuelUsed",
                table: "DriverSchedule",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DistanceCovered",
                table: "DriverSchedule",
                type: "decimal(10,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<long>(
                name: "DriverId1",
                table: "DriverSchedule",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxWindSpeed",
                table: "DriverRoutePreference",
                type: "decimal(5,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxPreferredWeight",
                table: "DriverRoutePreference",
                type: "decimal(10,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DriverId1",
                table: "DriverRoutePreference",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SafetyScore",
                table: "DriverPerformance",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "DriverPerformance",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OverallPerformanceScore",
                table: "DriverPerformance",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OnTimeDeliveryRate",
                table: "DriverPerformance",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "FuelEfficiency",
                table: "DriverPerformance",
                type: "decimal(8,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CustomerRating",
                table: "DriverPerformance",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<long>(
                name: "DriverId1",
                table: "DriverPerformance",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PayrollEntry",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RegularHours = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    OvertimeHours = table.Column<decimal>(type: "decimal(8,2)", nullable: false),
                    RegularRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OvertimeRate = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    RegularPay = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    OvertimePay = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    PerformanceBonus = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    SafetyBonus = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    OtherBonuses = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Deductions = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalCompensation = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalPay = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayrollEntry_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PayrollEntry_DriverId",
                table: "PayrollEntry",
                column: "DriverId");

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
                name: "FK_DriverSchedule_Loads_LoadId",
                table: "DriverSchedule",
                column: "LoadId",
                principalTable: "Loads",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Trucks_Drivers_AssignedDriverId",
                table: "Trucks",
                column: "AssignedDriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "FK_DriverSchedule_Loads_LoadId",
                table: "DriverSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Trucks_Drivers_AssignedDriverId",
                table: "Trucks");

            migrationBuilder.DropTable(
                name: "PayrollEntry");

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

            migrationBuilder.AlterColumn<decimal>(
                name: "FuelUsed",
                table: "DriverSchedule",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DistanceCovered",
                table: "DriverSchedule",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxWindSpeed",
                table: "DriverRoutePreference",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxPreferredWeight",
                table: "DriverRoutePreference",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "SafetyScore",
                table: "DriverPerformance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Rating",
                table: "DriverPerformance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OverallPerformanceScore",
                table: "DriverPerformance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "OnTimeDeliveryRate",
                table: "DriverPerformance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "FuelEfficiency",
                table: "DriverPerformance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "CustomerRating",
                table: "DriverPerformance",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRoutePreference_DriverId",
                table: "DriverRoutePreference",
                column: "DriverId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DriverSchedule_Loads_LoadId",
                table: "DriverSchedule",
                column: "LoadId",
                principalTable: "Loads",
                principalColumn: "Id");
        }
    }
}
