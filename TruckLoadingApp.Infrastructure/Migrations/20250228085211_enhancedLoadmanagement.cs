using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TruckLoadingApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class enhancedLoadmanagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SpecialRequirements",
                table: "Loads",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomsDeclarationNumber",
                table: "Loads",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlingInstructions",
                table: "Loads",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HazardousMaterialClass",
                table: "Loads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFragile",
                table: "Loads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresCustomsDeclaration",
                table: "Loads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresStackingControl",
                table: "Loads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresTemperatureControl",
                table: "Loads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StackingInstructions",
                table: "Loads",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UNNumber",
                table: "Loads",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LoadTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LoadTemperatureRequirements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoadId = table.Column<long>(type: "bigint", nullable: false),
                    MinTemperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    MaxTemperature = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    TemperatureUnit = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RequiresContinuousMonitoring = table.Column<bool>(type: "bit", nullable: false),
                    MonitoringIntervalMinutes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadTemperatureRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadTemperatureRequirements_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoadLoadTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoadId = table.Column<long>(type: "bigint", nullable: false),
                    LoadTagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoadLoadTags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoadLoadTags_LoadTags_LoadTagId",
                        column: x => x.LoadTagId,
                        principalTable: "LoadTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LoadLoadTags_Loads_LoadId",
                        column: x => x.LoadId,
                        principalTable: "Loads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loads_LoadTypeId",
                table: "Loads",
                column: "LoadTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Loads_RequiredTruckTypeId",
                table: "Loads",
                column: "RequiredTruckTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadLoadTags_LoadId_LoadTagId",
                table: "LoadLoadTags",
                columns: new[] { "LoadId", "LoadTagId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadLoadTags_LoadTagId",
                table: "LoadLoadTags",
                column: "LoadTagId");

            migrationBuilder.CreateIndex(
                name: "IX_LoadTags_Name",
                table: "LoadTags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LoadTemperatureRequirements_LoadId",
                table: "LoadTemperatureRequirements",
                column: "LoadId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Loads_LoadTypes_LoadTypeId",
                table: "Loads",
                column: "LoadTypeId",
                principalTable: "LoadTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Loads_TruckTypes_RequiredTruckTypeId",
                table: "Loads",
                column: "RequiredTruckTypeId",
                principalTable: "TruckTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loads_LoadTypes_LoadTypeId",
                table: "Loads");

            migrationBuilder.DropForeignKey(
                name: "FK_Loads_TruckTypes_RequiredTruckTypeId",
                table: "Loads");

            migrationBuilder.DropTable(
                name: "LoadLoadTags");

            migrationBuilder.DropTable(
                name: "LoadTemperatureRequirements");

            migrationBuilder.DropTable(
                name: "LoadTags");

            migrationBuilder.DropIndex(
                name: "IX_Loads_LoadTypeId",
                table: "Loads");

            migrationBuilder.DropIndex(
                name: "IX_Loads_RequiredTruckTypeId",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "CustomsDeclarationNumber",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "HandlingInstructions",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "HazardousMaterialClass",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "IsFragile",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "RequiresCustomsDeclaration",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "RequiresStackingControl",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "RequiresTemperatureControl",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "StackingInstructions",
                table: "Loads");

            migrationBuilder.DropColumn(
                name: "UNNumber",
                table: "Loads");

            migrationBuilder.AlterColumn<string>(
                name: "SpecialRequirements",
                table: "Loads",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
