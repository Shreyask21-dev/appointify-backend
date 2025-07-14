using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixConsultantShiftRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts",
                column: "PlanId",
                principalTable: "ConsultationPlans",
                principalColumn: "PlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts");

            migrationBuilder.AddForeignKey(
                name: "FK_ConsultantShifts_ConsultationPlans_PlanId",
                table: "ConsultantShifts",
                column: "PlanId",
                principalTable: "ConsultationPlans",
                principalColumn: "PlanId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
