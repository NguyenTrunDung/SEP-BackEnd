using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDerpartment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Sort",
                table: "Department",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sort",
                table: "Department");
        }
    }
}
