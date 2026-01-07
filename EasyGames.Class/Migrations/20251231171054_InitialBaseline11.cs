using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyGames.Class.Migrations
{
    /// <inheritdoc />
    public partial class InitialBaseline11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_StockItems_AvailableQuantity_LessOrEqual_Quantity",
                table: "StockItems",
                sql: "[AvailableQuantity] <= [Quantity]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_StockItems_AvailableQuantity_LessOrEqual_Quantity",
                table: "StockItems");
        }
    }
}
