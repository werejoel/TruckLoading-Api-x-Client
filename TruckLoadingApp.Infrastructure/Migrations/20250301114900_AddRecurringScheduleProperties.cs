using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRecurringScheduleProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstanceNumber",
                table: "DriverSchedule",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "RecurringScheduleId",
                table: "DriverSchedule",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "RecurringScheduleInstances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadId = table.Column<long>(type: "bigint", nullable: true),
                    InstanceNumber = table.Column<int>(type: "int", nullable: false),
                    IsModified = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringScheduleInstances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringScheduleInstances_DriverSchedule_ParentScheduleId",
                        column: x => x.ParentScheduleId,
                        principalTable: "DriverSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringScheduleInstances_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecurringScheduleInstances_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverSchedule_RecurringScheduleId",
                table: "DriverSchedule",
                column: "RecurringScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringScheduleInstances_DriverId",
                table: "RecurringScheduleInstances",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringScheduleInstances_LoadId",
                table: "RecurringScheduleInstances",
                column: "LoadId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringScheduleInstances_ParentScheduleId",
                table: "RecurringScheduleInstances",
                column: "ParentScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverSchedule_DriverSchedule_RecurringScheduleId",
                table: "DriverSchedule",
                column: "RecurringScheduleId",
                principalTable: "DriverSchedule",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverSchedule_DriverSchedule_RecurringScheduleId",
                table: "DriverSchedule");

            migrationBuilder.DropTable(
                name: "RecurringScheduleInstances");

            migrationBuilder.DropIndex(
                name: "IX_DriverSchedule_RecurringScheduleId",
                table: "DriverSchedule");

            migrationBuilder.DropColumn(
                name: "InstanceNumber",
                table: "DriverSchedule");

            migrationBuilder.DropColumn(
                name: "RecurringScheduleId",
                table: "DriverSchedule");
        }
    }
}
