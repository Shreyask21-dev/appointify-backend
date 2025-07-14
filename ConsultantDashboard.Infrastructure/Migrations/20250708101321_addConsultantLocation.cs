using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addConsultantLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LocationId",
                table: "PlanBufferRules",
                newName: "ConsultantLocationId");

            migrationBuilder.RenameIndex(
                name: "IX_PlanBufferRules_PlanId_LocationId",
                table: "PlanBufferRules",
                newName: "IX_PlanBufferRules_PlanId_ConsultantLocationId");

            migrationBuilder.CreateTable(
                name: "ConsultantLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsultantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_ConsultantLocations_ConsultantId",
                table: "ConsultantLocations",
                column: "ConsultantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConsultantLocations");

            migrationBuilder.RenameColumn(
                name: "ConsultantLocationId",
                table: "PlanBufferRules",
                newName: "LocationId");

            migrationBuilder.RenameIndex(
                name: "IX_PlanBufferRules_PlanId_ConsultantLocationId",
                table: "PlanBufferRules",
                newName: "IX_PlanBufferRules_PlanId_LocationId");
        }
    }
}
