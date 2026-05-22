using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLiBia.Migrations
{
    /// <inheritdoc />
    public partial class KhoiTaoDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TongTienGiờ",
                table: "HoaDons",
                newName: "TongTienGio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TongTienGio",
                table: "HoaDons",
                newName: "TongTienGiờ");
        }
    }
}
