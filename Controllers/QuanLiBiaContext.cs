using Microsoft.AspNetCore.Mvc;
using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using System.Linq;

namespace HeThongQuanLiBia.Controllers
{
    public class DichVuController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // 1. Hiển thị danh sách lọc theo loại hàng hóa
        public IActionResult Index(int loai = 0)
        {
            var items = _context.DichVus.Where(d => d.LoaiDichVu == loai).OrderBy(d => d.TenDichVu).ToList();
            ViewBag.LoaiHienTai = loai; // Gửi ngược dữ liệu sang View để đổi tiêu đề
            return View(items);
        }

        // 2. Giao diện thêm mới sản phẩm
        public IActionResult Create(int loai)
        {
            ViewBag.LoaiMacDinh = loai;
            return View();
        }

        // 3. Xử lý thêm mới sản phẩm vào SQL
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DichVu dv)
        {
            if (ModelState.IsValid)
            {
                _context.DichVus.Add(dv);
                _context.SaveChanges();
                TempData["Success"] = "Thêm sản phẩm dịch vụ thành công!";
                return RedirectToAction(nameof(Index), new { loai = dv.LoaiDichVu });
            }
            ViewBag.LoaiMacDinh = dv.LoaiDichVu;
            return View(dv);
        }

        // 4. Xử lý xóa sản phẩm
        [HttpPost]
        public IActionResult Delete(int id, int loai)
        {
            var dv = _context.DichVus.Find(id);
            if (dv != null)
            {
                _context.DichVus.Remove(dv);
                _context.SaveChanges();
                TempData["Success"] = "Xóa mặt hàng thành công!";
            }
            return RedirectToAction(nameof(Index), new { loai = loai });
        }
    }
}