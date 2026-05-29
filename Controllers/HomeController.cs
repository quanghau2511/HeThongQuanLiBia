using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HeThongQuanLiBia.Models;
using HeThongQuanLiBia.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HeThongQuanLiBia.Controllers;

[Authorize]
public class HomeController(ApplicationDbContext context) : Controller
{
  
    // 1. TRANG CHỦ POS
    //Kiemtra lại

    public IActionResult Index(int? id)
    {
        var danhSachBan = context.Bans.ToList();

        ViewBag.ListDichVu = context.DichVus.ToList();

        if (id.HasValue)
        {
            var banDangChon = context.Bans
                .FirstOrDefault(b => b.BanId == id.Value);

            ViewBag.BanDangChon = banDangChon;

            if (banDangChon != null)
            {
                var hoaDonHienTai = context.HoaDons
                    .Include(h => h.ChiTietDichVus)
                    .ThenInclude(ct => ct.DichVu)
                    .FirstOrDefault(h =>
                        h.BanId == banDangChon.BanId &&
                        h.GioRa == null);

                ViewBag.HoaDonHienTai = hoaDonHienTai;

                decimal tongCong = 0;

                if (hoaDonHienTai?.ChiTietDichVus != null)
                {
                    tongCong = hoaDonHienTai.ChiTietDichVus
                        .Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan);
                }

                ViewBag.TongCong = tongCong;
            }
        }

        return View(danhSachBan);
    }


    // 2. MỞ BÀN

    [HttpPost]
    public IActionResult MoBan(int id)
    {
        var ban = context.Bans
            .FirstOrDefault(b => b.BanId == id);

        if (ban != null && ban.TrangThai == 0)
        {
            ban.TrangThai = 1;

            var hoaDonMoi = new HoaDon
            {
                BanId = ban.BanId,
                GioVao = DateTime.Now,
                GioRa = null,
                TongTien = 0
            };

            context.HoaDons.Add(hoaDonMoi);

            context.SaveChanges();
        }

        return RedirectToAction(nameof(Index), new { id });
    }


    // 3. THÊM MÓN

    [HttpPost]
    public IActionResult ThemMon(int banId, int dichVuId)
    {
        var hoaDon = context.HoaDons
            .Include(h => h.ChiTietDichVus)
            .FirstOrDefault(h =>
                h.BanId == banId &&
                h.GioRa == null);

        var dichVu = context.DichVus
            .FirstOrDefault(dv => dv.DichVuId == dichVuId);

        if (hoaDon != null && dichVu != null)
        {
            var chiTiet = context.ChiTietDichVus
                .FirstOrDefault(ct =>
                    ct.HoaDonId == hoaDon.HoaDonId &&
                    ct.DichVuId == dichVuId);

            if (chiTiet != null)
            {
                chiTiet.SoLuong += 1;
            }
            else
            {
                var chiTietMoi = new ChiTietDichVu
                {
                    HoaDonId = hoaDon.HoaDonId,
                    DichVuId = dichVuId,
                    SoLuong = 1,
                    DonGiaTaiThoiDiemBan = dichVu.Gia
                };

                context.ChiTietDichVus.Add(chiTietMoi);
            }

            context.SaveChanges();
        }

        return RedirectToAction(nameof(Index), new { id = banId });
    }


    // 4. GIẢM MÓN

    [HttpPost]
    public IActionResult GiamMon(int banId, int dichVuId)
    {
        var hoaDon = context.HoaDons
            .Include(h => h.ChiTietDichVus)
            .FirstOrDefault(h =>
                h.BanId == banId &&
                h.GioRa == null);

        if (hoaDon != null)
        {
            var chiTiet = context.ChiTietDichVus
                .FirstOrDefault(ct =>
                    ct.HoaDonId == hoaDon.HoaDonId &&
                    ct.DichVuId == dichVuId);

            if (chiTiet != null)
            {
                if (chiTiet.SoLuong > 1)
                {
                    chiTiet.SoLuong -= 1;
                }
                else
                {
                    context.ChiTietDichVus.Remove(chiTiet);
                }

                context.SaveChanges();
            }
        }

        return RedirectToAction(nameof(Index), new { id = banId });
    }


    // 5. THANH TOÁN

    [HttpPost]
    public IActionResult ThanhToan(int banId)
    {
        var ban = context.Bans
            .FirstOrDefault(b => b.BanId == banId);

        var hoaDon = context.HoaDons
            .Include(h => h.Ban)
            .Include(h => h.ChiTietDichVus)
            .ThenInclude(ct => ct.DichVu)
            .FirstOrDefault(h =>
                h.BanId == banId &&
                h.GioRa == null);

        if (ban != null && hoaDon != null)
        {
            // cập nhật trạng thái bàn
            ban.TrangThai = 0;

            // giờ ra
            hoaDon.GioRa = DateTime.Now;

            // tiền dịch vụ
            decimal tienDichVu = hoaDon.ChiTietDichVus
                .Sum(ct => ct.SoLuong * ct.DonGiaTaiThoiDiemBan);

            // tiền giờ
            decimal giaMoiGio = 80000;

            TimeSpan thoiGianChoi =
                hoaDon.GioRa.Value - hoaDon.GioVao;

            double tongSoPhut =
                Math.Max(thoiGianChoi.TotalMinutes, 1);

            decimal tienGio =
                (decimal)(tongSoPhut / 60) * giaMoiGio;

            // tổng tiền
            hoaDon.TongTien = tienDichVu + tienGio;

            context.SaveChanges();

            ViewBag.TongCong = hoaDon.TongTien;

            // HIỆN HÓA ĐƠN
            return View("HoaDon", hoaDon);
        }

        return RedirectToAction(nameof(Index));
    }


    // 6. TRANG HÓA ĐƠN

    public IActionResult HoaDon(int id)
    {
        var hoaDon = context.HoaDons
            .Include(h => h.Ban)
            .Include(h => h.ChiTietDichVus)
            .ThenInclude(ct => ct.DichVu)
            .FirstOrDefault(h => h.HoaDonId == id);

        if (hoaDon == null)
        {
            return NotFound();
        }

        ViewBag.TongCong = hoaDon.TongTien;

        return View(hoaDon);
    }


    // 7. DOANH THU


    // 8. DASHBOARD
    [Authorize(Roles = "Admin")]
    public IActionResult Dashboard()
    {
        var today = DateTime.Today;
        var tomorrow = today.AddDays(1);

        var invoicesToday = context.HoaDons
            .Include(h => h.Ban)
            .Where(h => h.GioRa != null && h.GioRa >= today && h.GioRa < tomorrow)
            .ToList();

        var model = new DashboardViewModel
        {
            TotalRevenueToday = invoicesToday.Sum(h => h.TongTien),
            TotalInvoicesToday = invoicesToday.Count,
            TotalCustomersToday = invoicesToday.Select(h => h.BanId).Distinct().Count(),
            ActiveTables = context.Bans.Count(b => b.TrangThai == 1),
            RevenueChart = Enumerable.Range(0, 7)
                .Select(offset =>
                {
                    var date = today.AddDays(-offset);
                    var revenue = context.HoaDons
                        .Where(h => h.GioRa != null && h.GioRa >= date && h.GioRa < date.AddDays(1))
                        .Sum(h => h.TongTien);

                    return new ChartPoint
                    {
                        Label = date.ToString("dd/MM"),
                        Value = revenue
                    };
                })
                .Reverse()
                .ToList()
        };

        return View(model);
    }


    // 9. LỊCH SỬ HÓA ĐƠN
    [Authorize(Roles = "Admin")]
    public IActionResult InvoiceHistory(DateTime? searchDate, string? tableName, int? invoiceId)
    {
        var query = context.HoaDons
            .Include(h => h.Ban)
            .Where(h => h.GioRa != null);

        if (searchDate.HasValue)
        {
            query = query.Where(h => h.GioRa.HasValue && h.GioRa.Value.Date == searchDate.Value.Date);
        }

        if (!string.IsNullOrWhiteSpace(tableName))
        {
            query = query.Where(h => h.Ban != null && h.Ban.TenBan.Contains(tableName));
        }

        if (invoiceId.HasValue)
        {
            query = query.Where(h => h.HoaDonId == invoiceId.Value);
        }

        var model = new InvoiceHistoryViewModel
        {
            SearchDate = searchDate,
            TableName = tableName,
            InvoiceId = invoiceId,
            Invoices = query
                .OrderByDescending(h => h.GioRa)
                .ToList()
        };

        return View(model);
    }

    // 7. DOANH THU
    public IActionResult DoAccess(DateTime? fromDate, DateTime? toDate)
    {
        return RedirectToAction(nameof(DoanhThu),
            new { fromDate, toDate });
    }

    [Authorize(Roles = "Admin")]
    public IActionResult DoanhThu(DateTime? fromDate, DateTime? toDate)
    {
        var query = context.HoaDons
            .Include(h => h.Ban)
            .Where(h => h.GioRa != null);

        if (fromDate.HasValue)
        {
            query = query.Where(h =>
                h.GioVao >= fromDate.Value.Date);

            ViewBag.FromDate =
                fromDate.Value.ToString("yyyy-MM-dd");
        }

        if (toDate.HasValue)
        {
            var ngayKetThuc =
                toDate.Value.Date.AddDays(1).AddTicks(-1);

            query = query.Where(h =>
                h.GioVao <= ngayKetThuc);

            ViewBag.ToDate =
                toDate.Value.ToString("yyyy-MM-dd");
        }

        var danhSachHoaDon = query
            .OrderByDescending(h => h.GioRa)
            .ToList();

        return View(danhSachHoaDon);
    }
}