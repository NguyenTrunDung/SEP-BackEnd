using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HOMMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMealTimeAddMealTimeToFoodRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MealTime",
                table: "DiseaseCategoryFoodRestrictions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_MealTime",
                table: "DiseaseCategoryFoodRestrictions",
                column: "MealTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DiseaseCategoryFoodRestrictions_MealTime",
                table: "DiseaseCategoryFoodRestrictions");

            migrationBuilder.DropColumn(
                name: "MealTime",
                table: "DiseaseCategoryFoodRestrictions");
        }
    }
}
