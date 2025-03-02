using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class databaseupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CanTransportHazardousMaterials",
                table: "Trucks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyPhoneNumber",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CompanyRating",
                table: "Trucks",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "HasLiftgate",
                table: "Trucks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasLoadingRamp",
                table: "Trucks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasRefrigeration",
                table: "Trucks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "HazardousMaterialsClasses",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Trucks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "VolumeCapacity",
                table: "Trucks",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "Drivers",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "DriverDocument",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "FileSize",
                table: "DriverDocument",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IssuingAuthority",
                table: "DriverDocument",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "DriverDocument",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_CompanyId",
                table: "Drivers",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_AspNetUsers_CompanyId",
                table: "Drivers",
                column: "CompanyId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_AspNetUsers_CompanyId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_CompanyId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CanTransportHazardousMaterials",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "CompanyPhoneNumber",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "CompanyRating",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "HasLiftgate",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "HasLoadingRamp",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "HasRefrigeration",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "HazardousMaterialsClasses",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "VolumeCapacity",
                table: "Trucks");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "DriverDocument");

            migrationBuilder.DropColumn(
                name: "FileSize",
                table: "DriverDocument");

            migrationBuilder.DropColumn(
                name: "IssuingAuthority",
                table: "DriverDocument");

            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "DriverDocument");
        }
    }
}
