using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixBranchAndFoodRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Food_Categories_Branches_BranchId1",
                table: "Food_Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Foods_Branches_BranchId1",
                table: "Foods");

            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Branches_BranchId1",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_BranchId1",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Foods_BranchId1",
                table: "Foods");

            migrationBuilder.DropIndex(
                name: "IX_Food_Categories_BranchId1",
                table: "Food_Categories");

            migrationBuilder.DropColumn(
                name: "BranchId1",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "BranchId1",
                table: "Foods");

            migrationBuilder.DropColumn(
                name: "BranchId1",
                table: "Food_Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId1",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId1",
                table: "Foods",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BranchId1",
                table: "Food_Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_BranchId1",
                table: "Menus",
                column: "BranchId1");

            migrationBuilder.CreateIndex(
                name: "IX_Foods_BranchId1",
                table: "Foods",
                column: "BranchId1");

            migrationBuilder.CreateIndex(
                name: "IX_Food_Categories_BranchId1",
                table: "Food_Categories",
                column: "BranchId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Food_Categories_Branches_BranchId1",
                table: "Food_Categories",
                column: "BranchId1",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Foods_Branches_BranchId1",
                table: "Foods",
                column: "BranchId1",
                principalTable: "Branches",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Branches_BranchId1",
                table: "Menus",
                column: "BranchId1",
                principalTable: "Branches",
                principalColumn: "Id");
        }
    }
}
