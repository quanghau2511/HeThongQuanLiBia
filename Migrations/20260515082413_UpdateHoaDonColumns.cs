using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLiBia.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHoaDonColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TongTien",
                table: "HoaDons",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TongTien",
                table: "HoaDons");
        }
    }
}
