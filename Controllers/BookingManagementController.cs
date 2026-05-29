using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers;

[Authorize(Roles = "Admin,NhanVien,Staff")]
public class BookingManagementController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingManagementController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index(string search = "", int? status = null, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.DatBans
            .Include(d => d.KhachHang)
            .Include(d => d.Ban)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = search.Trim();
            query = query.Where(d =>
                d.KhachHang!.HoTen.Contains(keyword) ||
                d.KhachHang.SoDienThoai.Contains(keyword) ||
                d.Ban!.TenBan.Contains(keyword) ||
                (d.GhiChu != null && d.GhiChu.Contains(keyword)));
        }

        if (status.HasValue)
        {
            query = query.Where(d => d.TrangThai == status.Value);
        }

        if (from.HasValue)
        {
            query = query.Where(d => d.NgayDat >= from.Value.Date);
        }

        if (to.HasValue)
        {
            query = query.Where(d => d.NgayDat <= to.Value.Date);
        }

        var bookings = query
            .OrderByDescending(d => d.NgayDat)
            .ThenByDescending(d => d.GioBatDau)
            .ToList();

        ViewBag.Search = search;
        ViewBag.Status = status;
        ViewBag.From = from?.ToString("yyyy-MM-dd");
        ViewBag.To = to?.ToString("yyyy-MM-dd");
        ViewBag.TotalCount = bookings.Count;
        ViewBag.PendingCount = bookings.Count(d => d.TrangThai == 0);
        ViewBag.ConfirmedCount = bookings.Count(d => d.TrangThai == 1);
        ViewBag.CancelledCount = bookings.Count(d => d.TrangThai == 2);
        ViewBag.SuccessMessage = TempData["SuccessMessage"];

        return View(bookings);
    }

    public IActionResult Details(int id)
    {
        var booking = _context.DatBans
            .Include(d => d.KhachHang)
            .Include(d => d.Ban)
            .Include(d => d.DatBanDichVus)
            .ThenInclude(item => item.DichVu)
            .FirstOrDefault(d => d.DatBanId == id);

        if (booking == null)
        {
            return NotFound();
        }

        ViewBag.SuccessMessage = TempData["SuccessMessage"];
        return View(booking);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult UpdateStatus(int id, int status)
    {
        var booking = _context.DatBans.Find(id);
        if (booking == null)
        {
            return NotFound();
        }

        booking.TrangThai = status;
        _context.SaveChanges();

        TempData["SuccessMessage"] = status switch
        {
            1 => "Đã cập nhật trạng thái sang Đã xác nhận.",
            2 => "Đã cập nhật trạng thái sang Đã hủy.",
            _ => "Đã cập nhật trạng thái sang Chờ xác nhận."
        };

        return RedirectToAction(nameof(Index));
    }
}
