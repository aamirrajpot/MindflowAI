using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MindflowAI.Migrations
{
    /// <inheritdoc />
    public partial class EngeryLevel_DataType_Changed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "EnergyLevel",
                table: "WellnessCheckIns",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EnergyLevel",
                table: "WellnessCheckIns",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
