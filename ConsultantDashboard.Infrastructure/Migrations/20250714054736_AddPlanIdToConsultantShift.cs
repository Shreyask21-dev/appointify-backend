using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlanIdToConsultantShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PlanId",
                table: "ConsultantShifts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ConsultantShifts_PlanId",
                table: "ConsultantShifts",
                column: "PlanId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts",
                column: "PlanId",
                principalTable: "ConsultationPlans",
                principalColumn: "PlanId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts");

            migrationBuilder.DropIndex(
                name: "IX_ConsultantShifts_PlanId",
                table: "ConsultantShifts");

            migrationBuilder.DropColumn(
                name: "PlanId",
                table: "ConsultantShifts");
        }
    }
}
