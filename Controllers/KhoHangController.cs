using Microsoft.AspNetCore.Mvc;
using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;

namespace HeThongQuanLiBia.Controllers;

public class KhoHangController(ApplicationDbContext context) : Controller
{
    public IActionResult Index()
    {
        // Lấy danh sách hàng hóa sản phẩm, nếu null gán mảng rỗng [] theo C# 12
        var danhSachHang = context.DichVus?.ToList() ?? [];
        return View(danhSachHang);
    }

    [HttpPost]
    public IActionResult ThemMatHang(DichVu model)
    {
        if (ModelState.IsValid)
        {
            context.DichVus?.Add(model);
            context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        var danhSachHang = context.DichVus?.ToList() ?? [];
        return View(nameof(Index), danhSachHang);
    }
}