using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HourTrackerBackend.Migrations
{
    public partial class AddCalculatedTimeToProjectType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CalculatedTimeInSeconds",
                table: "ProjectTypes",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalculatedTimeInSeconds",
                table: "ProjectTypes");
        }
    }
}
