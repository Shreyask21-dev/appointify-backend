using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveWorkSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkStartTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSessions", x => x.Id);
                });
        }
    }
}
