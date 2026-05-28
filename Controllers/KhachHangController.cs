using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers;

[AllowAnonymous]
public class KhachHangController : Controller
{
    private readonly ApplicationDbContext _context;

    public KhachHangController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Register()
    {
        return View(new KhachHangRegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(KhachHangRegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (_context.KhachHangs.Any(k => k.Email.ToLower() == model.Email.Trim().ToLower()))
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng.");
            return View(model);
        }

        if (_context.KhachHangs.Any(k => k.SoDienThoai == model.SoDienThoai.Trim()))
        {
            ModelState.AddModelError(nameof(model.SoDienThoai), "Số điện thoại đã được sử dụng.");
            return View(model);
        }

        var khachHang = new KhachHang
        {
            HoTen = model.HoTen.Trim(),
            SoDienThoai = model.SoDienThoai.Trim(),
            Email = model.Email.Trim(),
            MatKhau = HashPassword(model.MatKhau),
            NgayTao = DateTime.Now
        };

        _context.KhachHangs.Add(khachHang);
        _context.SaveChanges();

        SaveCustomerSession(khachHang);
        TempData["SuccessMessage"] = "Đăng ký thành công! Chào mừng bạn đến với hệ thống đặt bàn.";

        return RedirectToAction(nameof(Dashboard));
    }

    public IActionResult Login()
    {
        return View(new KhachHangLoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(KhachHangLoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var khachHang = _context.KhachHangs
            .FirstOrDefault(k => k.Email.ToLower() == model.Email.Trim().ToLower());

        if (khachHang == null || !VerifyPassword(model.MatKhau, khachHang.MatKhau))
        {
            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }

        SaveCustomerSession(khachHang);
        TempData["SuccessMessage"] = $"Xin chào {khachHang.HoTen}!";

        return RedirectToAction(nameof(Dashboard));
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        TempData["SuccessMessage"] = "Bạn đã đăng xuất khỏi tài khoản khách hàng.";
        return RedirectToAction(nameof(Login));
    }

    public IActionResult Dashboard()
    {
        var khachHangId = HttpContext.Session.GetInt32("KhachHangId");
        if (khachHangId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var khachHang = _context.KhachHangs.Find(khachHangId.Value);
        if (khachHang == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        PrepareCustomerDashboard(khachHangId.Value, khachHang, null);
        return View(new DatBanBookingViewModel
        {
            NgayDat = DateTime.Today,
            GioBatDau = TimeOnly.FromDateTime(DateTime.Now),
            GioKetThuc = TimeOnly.FromDateTime(DateTime.Now.AddHours(2))
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DatBan(DatBanBookingViewModel model)
    {
        var khachHangId = HttpContext.Session.GetInt32("KhachHangId");
        if (khachHangId == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var khachHang = _context.KhachHangs.Find(khachHangId.Value);
        if (khachHang == null)
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        if (!ModelState.IsValid)
        {
            PrepareCustomerDashboard(khachHangId.Value, khachHang, model);
            return View("Dashboard", model);
        }

        var ban = _context.Bans.Find(model.BanId);
        if (ban == null)
        {
            ModelState.AddModelError(nameof(model.BanId), "Bàn không tồn tại.");
            PrepareCustomerDashboard(khachHangId.Value, khachHang, model);
            return View("Dashboard", model);
        }

        if (ban.TrangThai == 1)
        {
            ModelState.AddModelError(nameof(model.BanId), "Bàn đang có người chơi, vui lòng chọn bàn khác.");
            PrepareCustomerDashboard(khachHangId.Value, khachHang, model);
            return View("Dashboard", model);
        }

        if (model.GioKetThuc <= model.GioBatDau)
        {
            ModelState.AddModelError(nameof(model.GioKetThuc), "Giờ kết thúc phải sau giờ bắt đầu.");
            PrepareCustomerDashboard(khachHangId.Value, khachHang, model);
            return View("Dashboard", model);
        }

        var selectedItems = ParseSelectedServices(model.SelectedServicesJson);
        var overlap = _context.DatBans.Any(d =>
            d.BanId == model.BanId &&
            d.NgayDat.Date == model.NgayDat.Date &&
            d.TrangThai != 2 &&
            d.GioBatDau < model.GioKetThuc &&
            model.GioBatDau < d.GioKetThuc);

        if (overlap)
        {
            ModelState.AddModelError(string.Empty, "Khung giờ này đã có người đặt bàn. Vui lòng chọn thời gian khác.");
            PrepareCustomerDashboard(khachHangId.Value, khachHang, model);
            return View("Dashboard", model);
        }

        var datBan = new DatBan
        {
            KhachHangId = khachHangId.Value,
            BanId = model.BanId,
            NgayDat = model.NgayDat,
            GioBatDau = model.GioBatDau,
            GioKetThuc = model.GioKetThuc,
            GhiChu = model.GhiChu,
            TrangThai = 0
        };

        _context.DatBans.Add(datBan);
        _context.SaveChanges();

        var selectedDichVus = _context.DichVus
            .Where(d => selectedItems.Select(item => item.DichVuId).Contains(d.DichVuId))
            .ToDictionary(d => d.DichVuId, d => d);

        foreach (var item in selectedItems)
        {
            if (!selectedDichVus.TryGetValue(item.DichVuId, out var dichVu))
            {
                continue;
            }

            var datBanDichVu = new DatBanDichVu
            {
                DatBanId = datBan.DatBanId,
                DichVuId = item.DichVuId,
                SoLuong = item.SoLuong,
                DonGiaTaiThoiDiemBan = dichVu.Gia
            };

            _context.DatBanDichVus.Add(datBanDichVu);
        }

        _context.SaveChanges();

        var invoiceItems = _context.DatBanDichVus
            .Where(d => d.DatBanId == datBan.DatBanId)
            .Include(d => d.DichVu)
            .ToList();

        decimal dichVuTong = invoiceItems.Sum(item => item.SoLuong * item.DonGiaTaiThoiDiemBan);
        var duration = model.GioKetThuc.ToTimeSpan() - model.GioBatDau.ToTimeSpan();
        decimal gioGia = ban.GiaTheoGio > 0 ? ban.GiaTheoGio : 80000m;
        decimal tienGio = (decimal)(duration.TotalHours) * gioGia;
        decimal tongTien = dichVuTong + tienGio;

        var invoiceSummary = $"""
Bàn: {ban.TenBan}
Ngày đặt: {model.NgayDat:dd/MM/yyyy}
Giờ: {model.GioBatDau:hh\:mm} - {model.GioKetThuc:hh\:mm}
Tổng dịch vụ: {dichVuTong:N0} đ
Tiền giờ: {tienGio:N0} đ
TỔNG TẠM TÍNH: {tongTien:N0} đ
""";

        TempData["SuccessMessage"] = "Đặt bàn thành công!";
        TempData["InvoiceSummary"] = invoiceSummary;

        return RedirectToAction(nameof(Dashboard));
    }

    private void SaveCustomerSession(KhachHang khachHang)
    {
        HttpContext.Session.SetInt32("KhachHangId", khachHang.KhachHangId);
        HttpContext.Session.SetString("KhachHangTen", khachHang.HoTen);
        HttpContext.Session.SetString("KhachHangEmail", khachHang.Email);
    }

    private void PrepareCustomerDashboard(int khachHangId, KhachHang khachHang, DatBanBookingViewModel? bookingModel)
    {
        ViewBag.KhachHang = khachHang;
        ViewBag.BanList = _context.Bans.OrderBy(b => b.TenBan).ToList();
        ViewBag.DichVuList = _context.DichVus
            .OrderBy(d => d.LoaiDichVu)
            .ThenBy(d => d.TenDichVu)
            .ToList();
        ViewBag.LichSuDatBan = _context.DatBans
            .Where(d => d.KhachHangId == khachHangId)
            .Include(d => d.Ban)
            .Include(d => d.DatBanDichVus)
            .ThenInclude(item => item.DichVu)
            .OrderByDescending(d => d.NgayDat)
            .ThenByDescending(d => d.GioBatDau)
            .ToList();

        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        ViewBag.InvoiceSummary = TempData["InvoiceSummary"];

        if (bookingModel != null)
        {
            ViewBag.BookingModel = bookingModel;
        }
    }

    private List<SelectedServiceItem> ParseSelectedServices(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<List<SelectedServiceItem>>(json) ?? [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "|hls_billiards_customer"));
        return Convert.ToHexString(bytes);
    }

    private static bool VerifyPassword(string password, string storedHash)
    {
        return HashPassword(password) == storedHash;
    }
}
