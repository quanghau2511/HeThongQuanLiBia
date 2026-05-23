using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers
{
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // 1. TRANG CHỦ BÁN HÀNG
        public IActionResult Index(int? id)
        {
            var listBan = _context.Bans.ToList();
            var thoiGianChoi = new Dictionary<int, string>();

            // Tính thời gian chơi hiển thị trên card bàn
            foreach (var b in listBan.Where(x => x.TrangThai == 1))
            {
                var hd = _context.HoaDons.FirstOrDefault(h => h.BanId == b.BanId && !h.DaThanhToan);
                if (hd != null)
                {
                    var span = DateTime.Now - hd.GioVao;
                    thoiGianChoi[b.BanId] = $"{(int)span.TotalHours}h {span.Minutes}p";
                }
            }
            ViewBag.ThoiGianChoi = thoiGianChoi;
            ViewBag.ListDichVu = _context.DichVus.ToList();

            if (id.HasValue)
            {
                var banChon = _context.Bans.Find(id);
                if (banChon != null)
                {
                    ViewBag.BanDangChon = banChon;
                    var hoaDon = _context.HoaDons
                        .Include(h => h.ChiTietDichVus).ThenInclude(ct => ct.DichVu)
                        .FirstOrDefault(h => h.BanId == id && !h.DaThanhToan);

                    if (hoaDon != null)
                    {
                        ViewBag.HoaDonHienTai = hoaDon;

                        // Tính tiền dịch vụ (dùng ?? 0m để tránh null)
                        decimal tongDichVu = hoaDon.ChiTietDichVus?.Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan) ?? 0m;

                        // Tính tiền giờ (đảm bảo chia cho 60.0 để ra số thực)
                        double soPhut = Math.Max(0, (DateTime.Now - hoaDon.GioVao).TotalMinutes);
                        decimal tienGio = Math.Round((decimal)(soPhut / 60.0) * banChon.GiaTheoGio, 0);

                        ViewBag.TienDichVu = tongDichVu;
                        ViewBag.TienGioTamTinh = tienGio;
                        ViewBag.TongCong = tongDichVu + tienGio;
                    }
                }
            }
            return View(listBan);
        }

        // 2. CÁC HÀM XỬ LÝ DỮ LIỆU (Giữ nguyên logic của bạn vì đã rất tốt)
        [HttpPost]
        public IActionResult ThemMon(int banId, int dichVuId)
        {
            var hd = _context.HoaDons.FirstOrDefault(h => h.BanId == banId && !h.DaThanhToan);
            if (hd == null) return RedirectToAction("Index", new { id = banId });

            var dv = _context.DichVus.Find(dichVuId);
            if (dv == null) return RedirectToAction("Index", new { id = banId });

            var ct = _context.ChiTietDichVus.FirstOrDefault(c => c.HoaDonId == hd.HoaDonId && c.DichVuId == dichVuId);
            if (ct != null) ct.SoLuong += 1;
            else _context.ChiTietDichVus.Add(new ChiTietDichVu { HoaDonId = hd.HoaDonId, DichVuId = dichVuId, SoLuong = 1, DonGiaTaiThoiDiemBan = dv.Gia });

            _context.SaveChanges();
            return RedirectToAction("Index", new { id = banId });
        }

        [HttpPost]
        public IActionResult ThanhToan(int banId)
        {
            var hoaDon = _context.HoaDons.Include(h => h.ChiTietDichVus)
                .FirstOrDefault(h => h.BanId == banId && !h.DaThanhToan);
            var ban = _context.Bans.Find(banId);

            if (hoaDon == null || ban == null) return RedirectToAction("Index");

            DateTime gioRa = DateTime.Now;
            double soPhutChoi = Math.Max(0, (gioRa - hoaDon.GioVao).TotalMinutes);
            decimal tienGio = Math.Round((decimal)(soPhutChoi / 60.0) * ban.GiaTheoGio, 0);
            decimal tongDichVu = hoaDon.ChiTietDichVus?.Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan) ?? 0m;

            hoaDon.GioRa = gioRa;
            hoaDon.TongTien = tongDichVu + tienGio;
            hoaDon.DaThanhToan = true;
            ban.TrangThai = 0;

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // ... (Các hàm MoBan, DoanhThu giữ nguyên như của bạn)
    }
}