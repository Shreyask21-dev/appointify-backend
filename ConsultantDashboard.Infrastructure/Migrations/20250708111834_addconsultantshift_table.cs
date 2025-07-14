using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addconsultantshift_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultantLocations");

            migrationBuilder.DropTable(
                name: "PlanBufferRules");

            migrationBuilder.CreateTable(
                name: "Shifts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shifts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlanShiftBufferRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShiftId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BufferInMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanShiftBufferRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlanShiftBufferRules_ConsultationPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "ConsultationPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlanShiftBufferRules_Shifts_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "Shifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlanShiftBufferRules_PlanId_ShiftId",
                table: "PlanShiftBufferRules",
                columns: new[] { "PlanId", "ShiftId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanShiftBufferRules_ShiftId",
                table: "PlanShiftBufferRules",
                column: "ShiftId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlanShiftBufferRules");

            migrationBuilder.DropTable(
                name: "Shifts");

            migrationBuilder.CreateTable(
                name: "ConsultantLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsultantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultantLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConsultantLocations_ConsultantProfile_ConsultantId",
                        column: x => x.ConsultantId,
                        principalTable: "ConsultantProfile",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlanBufferRules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BufferInMinutes = table.Column<int>(type: "int", nullable: false),
                    ConsultantLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlanBufferRules", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultantLocations_ConsultantId",
                table: "ConsultantLocations",
                column: "ConsultantId");

            migrationBuilder.CreateIndex(
                name: "IX_PlanBufferRules_PlanId_ConsultantLocationId",
                table: "PlanBufferRules",
                columns: new[] { "PlanId", "ConsultantLocationId" },
                unique: true);
        }
    }
}
