using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KittyShop.Data.Migrations
{
    public partial class seedusers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Password", "Type", "UserName" },
                values: new object[] { 1, null, "CfDJ8Kd3fbsdAEdLsODDeNf8c5r7gSLI-Lx8tRTK9uPUJEa86c1m4O7G_A7QND-DqBGcdHr8E_UvAJcNFm41K6pTfkfwUoHf9mUn4kn9hsxapHjy5H4Wa8Z-8Si4AcunyPOpfA", 1, "Admin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Email", "Password", "Type", "UserName" },
                values: new object[] { 2, null, "CfDJ8Kd3fbsdAEdLsODDeNf8c5r-RXW2JG7G_JvR6Wl2-pSS_MqG_QLanSUC6qWkWsMvN2FPNLLQw0TWUBwN0nMLWqlXd0T63Bst6DxnYIeDUgkHxIsiyzlcB6l7lKDyK-xgCQ", 0, "Customer" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);
        }
    }
}
