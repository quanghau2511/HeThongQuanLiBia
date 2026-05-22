using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers
{
    // Áp dụng Primary Constructor: Đưa trực tiếp ApplicationDbContext lên tên Class
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // 1. TRANG CHỦ BÁN HÀNG & TÍNH TIỀN TẠM TÍNH
        public IActionResult Index(int? id)
        {
            var listBan = _context.Bans.ToList();
            var thoiGianChoi = new Dictionary<int, string>();

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

            // Lấy danh sách dịch vụ
            ViewBag.ListDichVu = _context.DichVus.ToList();

            if (id.HasValue)
            {
                var banChon = _context.Bans.Find(id);
                if (banChon != null)
                {
                    ViewBag.BanDangChon = banChon;
                    var hoaDon = _context.HoaDons
                        .Include(h => h.ChiTietDichVus)
                        .ThenInclude(ct => ct.DichVu)
                        .FirstOrDefault(h => h.BanId == id && !h.DaThanhToan);

                    if (hoaDon != null)
                    {
                        ViewBag.HoaDonHienTai = hoaDon;

                        // Tính tổng tiền dịch vụ đang gọi
                        decimal tongDichVu = hoaDon.ChiTietDichVus?.Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan) ?? 0m;
                        ViewBag.TienDichVu = tongDichVu;

                        // Tính tiền giờ tạm tính đến thời điểm hiện tại
                        double soPhut = (DateTime.Now - hoaDon.GioVao).TotalMinutes;
                        if (soPhut < 0) soPhut = 0;
                        decimal tienGio = Math.Round((decimal)(soPhut / 60) * banChon.GiaTheoGio, 0);
                        ViewBag.TienGioTamTinh = tienGio;

                        // Đảm bảo cả 2 đều là decimal thuần trước khi cộng
                        ViewBag.TongCong = tongDichVu + tienGio;
                    }
                }
            }
            return View(listBan);
        }

        // 2. THÊM MÓN / DỊCH VỤ VÀO BÀN BIA
        [HttpPost]
        public IActionResult ThemMon(int banId, int dichVuId)
        {
            var hd = _context.HoaDons.FirstOrDefault(h => h.BanId == banId && !h.DaThanhToan);
            if (hd == null) return RedirectToAction("Index", new { id = banId });

            var dv = _context.DichVus.Find(dichVuId);
            if (dv == null) return RedirectToAction("Index", new { id = banId });

            var ct = _context.ChiTietDichVus.FirstOrDefault(c => c.HoaDonId == hd.HoaDonId && c.DichVuId == dichVuId);
            if (ct != null)
            {
                ct.SoLuong += 1;
            }
            else
            {
                _context.ChiTietDichVus.Add(new ChiTietDichVu
                {
                    HoaDonId = hd.HoaDonId,
                    DichVuId = dichVuId,
                    SoLuong = 1,
                    DonGiaTaiThoiDiemBan = dv.Gia
                });
            }
            _context.SaveChanges();
            return RedirectToAction("Index", new { id = banId });
        }

        // 3. TĂNG GIẢM SỐ LƯỢNG MÓN TRONG HÓA ĐƠN
        [HttpPost]
        public IActionResult CapNhatMon(int banId, int dichVuId, int delta)
        {
            var hd = _context.HoaDons.FirstOrDefault(h => h.BanId == banId && !h.DaThanhToan);
            if (hd == null) return RedirectToAction("Index", new { id = banId });

            var ct = _context.ChiTietDichVus.FirstOrDefault(c => c.HoaDonId == hd.HoaDonId && c.DichVuId == dichVuId);

            if (ct != null)
            {
                ct.SoLuong += delta;
                if (ct.SoLuong <= 0)
                {
                    _context.ChiTietDichVus.Remove(ct);
                }
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { id = banId });
        }

        // 4. XỬ LÝ MỞ BÀN MỚI
        [HttpPost]
        public IActionResult MoBan(int banId)
        {
            var ban = _context.Bans.FirstOrDefault(b => b.BanId == banId);
            if (ban == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin bàn!";
                return RedirectToAction("Index");
            }

            if (ban.TrangThai == 1)
            {
                TempData["Error"] = "Bàn này đang có khách chơi rồi!";
                return RedirectToAction("Index", new { id = banId });
            }

            ban.TrangThai = 1;

            var hoaDonMoi = new HoaDon
            {
                BanId = banId,
                GioVao = DateTime.Now,
                DaThanhToan = false,
                TongTien = 0m
            };

            _context.HoaDons.Add(hoaDonMoi);
            _context.SaveChanges();

            TempData["Success"] = $"Mở thành công {ban.TenBan}! Giờ vào: {hoaDonMoi.GioVao.ToString("HH:mm")}";
            return RedirectToAction("Index", new { id = banId });
        }

        // 5. XỬ LÝ THANH TOÁN & CHỐT HÓA ĐƠN
        [HttpPost]
        public IActionResult ThanhToan(int banId)
        {
            var hoaDon = _context.HoaDons
                .Include(h => h.ChiTietDichVus)
                .FirstOrDefault(h => h.BanId == banId && !h.DaThanhToan);

            if (hoaDon == null)
            {
                TempData["Error"] = "Không tìm thấy hóa đơn chưa thanh toán cho bàn này!";
                return RedirectToAction("Index", new { id = banId });
            }

            var ban = _context.Bans.FirstOrDefault(b => b.BanId == banId);
            if (ban == null)
            {
                TempData["Error"] = "Không tìm thấy thông tin bàn!";
                return RedirectToAction("Index");
            }

            DateTime gioRa = DateTime.Now;
            hoaDon.GioRa = gioRa;

            double soPhutChoi = (gioRa - hoaDon.GioVao).TotalMinutes;
            if (soPhutChoi < 0) soPhutChoi = 0;

            decimal tienGio = Math.Round((decimal)(soPhutChoi / 60.0) * (decimal)ban.GiaTheoGio, 0);
                        decimal tongDichVu = hoaDon.ChiTietDichVus?.Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan) ?? 0m;

            // Chốt tổng tiền (TongTien là decimal không nullable)
            hoaDon.TongTien = tongDichVu + tienGio;

            hoaDon.DaThanhToan = true;
            ban.TrangThai = 0;

            _context.SaveChanges();

            TempData["Success"] = $"Thanh toán {ban.TenBan} thành công! Tổng tiền: {hoaDon.TongTien.ToString("N0")} VNĐ";
            return RedirectToAction("Index");
        }

        // 6. TRANG THỐNG KÊ DOANH THU (AN TOÀN KIỂU DỮ LIỆU)
        public IActionResult DoanhThu(DateTime? tuNgay, DateTime? denNgay)
        {
            if (!tuNgay.HasValue) tuNgay = DateTime.Today;
            if (!denNgay.HasValue) denNgay = DateTime.Today.AddDays(1).AddTicks(-1);
            else denNgay = denNgay.Value.Date.AddDays(1).AddTicks(-1);

            // Nạp kèm ChiTietDichVus và thông tin Bàn để tính toán chính xác dữ liệu lịch sử
            var danhSachHD = _context.HoaDons
                .Include(h => h.Ban)
                .Include(h => h.ChiTietDichVus)
                .Where(h => h.DaThanhToan && h.GioRa >= tuNgay && h.GioRa <= denNgay)
                .OrderByDescending(h => h.GioRa)
                .ToList();

            // Tính toán doanh thu (TongTien là decimal không nullable)
            decimal tongDoanhThu = danhSachHD.Sum(h => h.TongTien);

            // Tính toán bóc tách nâng cao để hiển thị ra View
            decimal tongTienDichVu = danhSachHD.Sum(h => h.ChiTietDichVus?.Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan) ?? 0m);
            decimal tongTienGio = tongDoanhThu - tongTienDichVu;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.TongDoanhThu = tongDoanhThu;
            ViewBag.TongTienDichVu = tongTienDichVu;
            ViewBag.TongTienGio = tongTienGio;
            ViewBag.SoLuongHoaDon = danhSachHD.Count;

            return View(danhSachHD);
        }
    }
}