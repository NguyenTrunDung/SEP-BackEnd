using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBranchIdFromBranchRoles_ManuallyApplied : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchRoles_Branches_BranchId",
                table: "BranchRoles");

            migrationBuilder.DropIndex(
                name: "IX_BranchRoles_BranchId",
                table: "BranchRoles");

            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "BranchRoles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BranchId",
                table: "BranchRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_BranchRoles_BranchId",
                table: "BranchRoles",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchRoles_Branches_BranchId",
                table: "BranchRoles",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
