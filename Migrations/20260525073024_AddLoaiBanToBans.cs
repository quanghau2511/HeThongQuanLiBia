using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLiBia.Migrations
{
    /// <inheritdoc />
    public partial class AddLoaiBanToBans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoaiBan",
                table: "Bans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LoaiBan",
                table: "Bans");
        }
    }
}
