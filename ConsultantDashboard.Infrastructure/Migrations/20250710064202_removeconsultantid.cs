using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removeconsultantid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsultantId",
                table: "ConsultantShifts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ConsultantId",
                table: "ConsultantShifts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
