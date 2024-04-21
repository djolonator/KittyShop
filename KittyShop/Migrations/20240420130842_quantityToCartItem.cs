using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KittyShop.Migrations
{
    public partial class quantityToCartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "CartItems");
        }
    }
}
