using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddManyToManyConsultationPlanShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts");

            migrationBuilder.DropIndex(
                name: "IX_ConsultantShifts_PlanId",
                table: "ConsultantShifts");

            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "ConsultationPlans");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "ConsultantShifts");

            migrationBuilder.CreateTable(
                name: "ConsultantPlanShifts",
                columns: table => new
                {
                    ConsultantShiftsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlansPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultantPlanShifts", x => new { x.ConsultantShiftsId, x.PlansPlanId });
                    table.ForeignKey(
                        name: "FK_ConsultantPlanShifts_ConsultantShifts_ConsultantShiftsId",
                        column: x => x.ConsultantShiftsId,
                        principalTable: "ConsultantShifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConsultantPlanShifts_ConsultationPlans_PlansPlanId",
                        column: x => x.PlansPlanId,
                        principalTable: "ConsultationPlans",
                        principalColumn: "PlanId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultantPlanShifts_PlansPlanId",
                table: "ConsultantPlanShifts",
                column: "PlansPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultantPlanShifts");

            migrationBuilder.AddColumn<Guid>(
                name: "ShiftId",
                table: "ConsultationPlans",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "ConsultantShifts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConsultantShifts_PlanId",
                table: "ConsultantShifts",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts",
                column: "PlanId",
                principalTable: "ConsultationPlans",
                principalColumn: "PlanId");
        }
    }
}
