using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updated1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanShiftBufferRules_Shifts_ShiftId",
                table: "PlanShiftBufferRules");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.CreateTable(
                name: "ConsultantShifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultantShifts", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PlanShiftBufferRules_ConsultantShifts_ShiftId",
                table: "PlanShiftBufferRules",
                column: "ShiftId",
                principalTable: "ConsultantShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlanShiftBufferRules_ConsultantShifts_ShiftId",
                table: "PlanShiftBufferRules");

            migrationBuilder.DropTable(
                name: "ConsultantShifts");

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_PlanShiftBufferRules_Shifts_ShiftId",
                table: "PlanShiftBufferRules",
                column: "ShiftId",
                principalTable: "Shifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
