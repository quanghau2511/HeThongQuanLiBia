using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HeThongQuanLiBia.Migrations
{
    /// <inheritdoc />
    public partial class AddKhachHangAndDatBan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KhachHangs",
                columns: table => new
                {
                    KhachHangId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHangs", x => x.KhachHangId);
                });

            migrationBuilder.CreateTable(
                name: "DatBans",
                columns: table => new
                {
                    DatBanId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KhachHangId = table.Column<int>(type: "int", nullable: false),
                    BanId = table.Column<int>(type: "int", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: false),
                    GhiChu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatBans", x => x.DatBanId);
                    table.ForeignKey(
                        name: "FK_DatBans_Bans_BanId",
                        column: x => x.BanId,
                        principalTable: "Bans",
                        principalColumn: "BanId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DatBans_KhachHangs_KhachHangId",
                        column: x => x.KhachHangId,
                        principalTable: "KhachHangs",
                        principalColumn: "KhachHangId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DatBanDichVus",
                columns: table => new
                {
                    DatBanDichVuId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DatBanId = table.Column<int>(type: "int", nullable: false),
                    DichVuId = table.Column<int>(type: "int", nullable: false),
                    SoLuong = table.Column<int>(type: "int", nullable: false),
                    DonGiaTaiThoiDiemBan = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatBanDichVus", x => x.DatBanDichVuId);
                    table.ForeignKey(
                        name: "FK_DatBanDichVus_DatBans_DatBanId",
                        column: x => x.DatBanId,
                        principalTable: "DatBans",
                        principalColumn: "DatBanId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DatBanDichVus_DichVus_DichVuId",
                        column: x => x.DichVuId,
                        principalTable: "DichVus",
                        principalColumn: "DichVuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatBanDichVus_DatBanId",
                table: "DatBanDichVus",
                column: "DatBanId");

            migrationBuilder.CreateIndex(
                name: "IX_DatBanDichVus_DichVuId",
                table: "DatBanDichVus",
                column: "DichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_DatBans_BanId",
                table: "DatBans",
                column: "BanId");

            migrationBuilder.CreateIndex(
                name: "IX_DatBans_KhachHangId",
                table: "DatBans",
                column: "KhachHangId");

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_Email",
                table: "KhachHangs",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_KhachHangs_SoDienThoai",
                table: "KhachHangs",
                column: "SoDienThoai",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatBanDichVus");

            migrationBuilder.DropTable(
                name: "DatBans");

            migrationBuilder.DropTable(
                name: "KhachHangs");
        }
    }
}
