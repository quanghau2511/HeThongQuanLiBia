using HeThongQuanLiBia.Models;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Bảng quản lý bàn bida của bạn
        public DbSet<Bans> Bans { get; set; }

        // Bảng quản lý hóa đơn và chi tiết dịch vụ
        public DbSet<HoaDon> HoaDons { get; set; }
        public DbSet<ChiTietDichVu> ChiTietDichVus { get; set; }

        // Bảng quản lý dịch vụ (Thêm dòng này vào để hết lỗi)
        // Lưu ý: Nếu Model của bạn đặt tên là DichVu thì để là <DichVu>, nếu đặt là DichVus thì để là <DichVus> nhé
        public DbSet<DichVu> DichVus { get; set; }

        // Bảng tài khoản đăng nhập
        public DbSet<TaiKhoan> TaiKhoans { get; set; }

        // Khách hàng đặt bàn online
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<DatBan> DatBans { get; set; }
        public DbSet<DatBanDichVu> DatBanDichVus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaiKhoan>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<KhachHang>()
                .HasIndex(k => k.Email)
                .IsUnique();

            modelBuilder.Entity<KhachHang>()
                .HasIndex(k => k.SoDienThoai)
                .IsUnique();

            modelBuilder.Entity<KhachHang>()
                .HasMany(k => k.DatBans)
                .WithOne(d => d.KhachHang)
                .HasForeignKey(d => d.KhachHangId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatBan>()
                .HasOne(d => d.Ban)
                .WithMany()
                .HasForeignKey(d => d.BanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DatBan>()
                .HasMany(d => d.DatBanDichVus)
                .WithOne(dd => dd.DatBan)
                .HasForeignKey(dd => dd.DatBanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DatBanDichVu>()
                .HasOne(dd => dd.DichVu)
                .WithMany()
                .HasForeignKey(dd => dd.DichVuId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaiKhoan>().HasData(
                new TaiKhoan
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123",
                    Role = "Admin"
                },
                new TaiKhoan
                {
                    Id = 2,
                    Username = "nhanvien",
                    Password = "nv123",
                    Role = "NhanVien"
                });
        }
    }
}