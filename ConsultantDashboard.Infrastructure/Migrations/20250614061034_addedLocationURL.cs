using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsultantDashboard.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedLocationURL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IFrameURL",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IFrameURL",
                table: "Locations");
        }
    }
}
