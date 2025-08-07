using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Branches_BranchId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Orders_OrderId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Comment",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "FoodId",
                table: "Comment",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comment_FoodId",
                table: "Comment",
                column: "FoodId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Branches_BranchId",
                table: "Comment",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Foods_FoodId",
                table: "Comment",
                column: "FoodId",
                principalTable: "Foods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Orders_OrderId",
                table: "Comment",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Branches_BranchId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Foods_FoodId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Orders_OrderId",
                table: "Comment");

            migrationBuilder.DropForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_FoodId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "FoodId",
                table: "Comment");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Branches_BranchId",
                table: "Comment",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Orders_OrderId",
                table: "Comment",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comment_Users_UserId",
                table: "Comment",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
