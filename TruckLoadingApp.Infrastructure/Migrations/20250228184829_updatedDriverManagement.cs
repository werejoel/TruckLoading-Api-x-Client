using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedDriverManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "AssignedDriverId",
                table: "Trucks",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Region",
                table: "Loads",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Drivers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Drivers",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DriverCertification",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    CertificationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RenewalDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverCertification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverCertification_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverDocument",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DocumentNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerificationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverDocument", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverDocument_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverPerformance",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SafetyIncidents = table.Column<int>(type: "int", nullable: false),
                    SafetyViolations = table.Column<int>(type: "int", nullable: false),
                    SpeedingEvents = table.Column<int>(type: "int", nullable: false),
                    HarshBrakingEvents = table.Column<int>(type: "int", nullable: false),
                    HarshAccelerationEvents = table.Column<int>(type: "int", nullable: false),
                    OnTimeDeliveries = table.Column<int>(type: "int", nullable: false),
                    LateDeliveries = table.Column<int>(type: "int", nullable: false),
                    OnTimeDeliveryRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDeliveries = table.Column<int>(type: "int", nullable: false),
                    CustomerRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FuelEfficiency = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalDrivingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    TotalRestTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    RestBreakViolations = table.Column<int>(type: "int", nullable: false),
                    HoursOfServiceViolations = table.Column<int>(type: "int", nullable: false),
                    VehicleInspectionScore = table.Column<int>(type: "int", nullable: false),
                    MaintenanceIssuesReported = table.Column<int>(type: "int", nullable: false),
                    OverallPerformanceScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PerformanceNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SafetyScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverPerformance", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverPerformance_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverRestPeriod",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCompliant = table.Column<bool>(type: "bit", nullable: false),
                    ComplianceNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverRestPeriod", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverRestPeriod_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverRoutePreference",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    PreferredRegions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvoidedRegions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferHighways = table.Column<bool>(type: "bit", nullable: false),
                    AvoidTolls = table.Column<bool>(type: "bit", nullable: false),
                    AvoidFerries = table.Column<bool>(type: "bit", nullable: false),
                    AvoidUrbanCenters = table.Column<bool>(type: "bit", nullable: false),
                    PreferredStartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    PreferredEndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    AvoidNightDriving = table.Column<bool>(type: "bit", nullable: false),
                    AvoidPeakHours = table.Column<bool>(type: "bit", nullable: false),
                    MaxPreferredWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PreferredLoadTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvoidedLoadTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvoidSevereWeather = table.Column<bool>(type: "bit", nullable: false),
                    MaxWindSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AvoidSnowRoutes = table.Column<bool>(type: "bit", nullable: false),
                    PreferredRestStopInterval = table.Column<int>(type: "int", nullable: false),
                    PreferredRestStopAmenities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimePriority = table.Column<int>(type: "int", nullable: false),
                    SafetyPriority = table.Column<int>(type: "int", nullable: false),
                    FuelEfficiencyPriority = table.Column<int>(type: "int", nullable: false),
                    ComfortPriority = table.Column<int>(type: "int", nullable: false),
                    AdditionalNotes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverRoutePreference", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverRoutePreference_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DriverSchedule",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverId = table.Column<long>(type: "bigint", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoadId = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsRecurring = table.Column<bool>(type: "bit", nullable: false),
                    RecurrencePattern = table.Column<int>(type: "int", nullable: true),
                    RecurrenceEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DistanceCovered = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FuelUsed = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverSchedule_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DriverSchedule_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverCertification_DriverId",
                table: "DriverCertification",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverDocument_DriverId",
                table: "DriverDocument",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverPerformance_DriverId",
                table: "DriverPerformance",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRestPeriod_DriverId",
                table: "DriverRestPeriod",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverRoutePreference_DriverId",
                table: "DriverRoutePreference",
                column: "DriverId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DriverSchedule_DriverId",
                table: "DriverSchedule",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverSchedule_LoadId",
                table: "DriverSchedule",
                column: "LoadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverCertification");

            migrationBuilder.DropTable(
                name: "DriverDocument");

            migrationBuilder.DropTable(
                name: "DriverPerformance");

            migrationBuilder.DropTable(
                name: "DriverRestPeriod");

            migrationBuilder.DropTable(
                name: "DriverRoutePreference");

            migrationBuilder.DropTable(
                name: "DriverSchedule");

            migrationBuilder.DropColumn(
                name: "AssignedDriverId",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "Region",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Drivers");

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);
        }
    }
}
