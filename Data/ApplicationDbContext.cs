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
    }
}