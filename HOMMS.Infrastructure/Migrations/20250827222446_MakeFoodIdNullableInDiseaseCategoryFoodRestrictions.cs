using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeFoodIdNullableInDiseaseCategoryFoodRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_DiseaseCategoryId_FoodId",
                table: "DiseaseCategoryFoodRestrictions");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "DiseaseCategoryFoodRestrictions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_DiseaseCategoryId",
                table: "DiseaseCategoryFoodRestrictions",
                column: "DiseaseCategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_DiseaseCategoryId",
                table: "DiseaseCategoryFoodRestrictions");

            migrationBuilder.AlterColumn<int>(
                name: "FoodId",
                table: "DiseaseCategoryFoodRestrictions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_DiseaseCategoryId_FoodId",
                table: "DiseaseCategoryFoodRestrictions",
                columns: new[] { "DiseaseCategoryId", "FoodId" },
                unique: true);
        }
    }
}
