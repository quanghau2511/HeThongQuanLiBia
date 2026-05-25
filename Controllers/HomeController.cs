using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers
{
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public IActionResult Index(int? id)
        {
            var listBan = _context.Bans.ToList();
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
                        decimal tongDichVu = hoaDon.ChiTietDichVus?.Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan) ?? 0m;
                        double soPhut = Math.Max(0, (DateTime.Now - hoaDon.GioVao).TotalMinutes);
                        decimal tienGio = Math.Round((decimal)(soPhut / 60.0) * banChon.GiaTheoGio, 0);
                        ViewBag.TongCong = tongDichVu + tienGio;
                    }
                }
            }
            return View(listBan);
        }

        [HttpPost]
        public IActionResult MoBan(int id)
        {
            var ban = _context.Bans.Find(id);
            if (ban == null) return NotFound();
            ban.TrangThai = 1;
            _context.HoaDons.Add(new HoaDon { BanId = id, GioVao = DateTime.Now, DaThanhToan = false });
            _context.SaveChanges();
            return RedirectToAction("Index", new { id = id });
        }

        [HttpPost]
        public IActionResult ThemMon(int banId, int dichVuId)
        {
            var hd = _context.HoaDons.FirstOrDefault(h => h.BanId == banId && !h.DaThanhToan);
            var dv = _context.DichVus.Find(dichVuId);
            if (hd != null && dv != null)
            {
                var ct = _context.ChiTietDichVus.FirstOrDefault(c => c.HoaDonId == hd.HoaDonId && c.DichVuId == dichVuId);
                if (ct != null) ct.SoLuong += 1;
                else _context.ChiTietDichVus.Add(new ChiTietDichVu { HoaDonId = hd.HoaDonId, DichVuId = dichVuId, SoLuong = 1, DonGiaTaiThoiDiemBan = dv.Gia });
                _context.SaveChanges();
            }
            return RedirectToAction("Index", new { id = banId });
        }

        [HttpPost]
        public IActionResult ThanhToan(int banId)
        {
            var hoaDon = _context.HoaDons.Include(h => h.ChiTietDichVus).FirstOrDefault(h => h.BanId == banId && !h.DaThanhToan);
            var ban = _context.Bans.Find(banId);
            if (hoaDon != null && ban != null)
            {
                hoaDon.GioRa = DateTime.Now;
                hoaDon.DaThanhToan = true;
                ban.TrangThai = 0;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}