using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DichVusController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DichVusController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TRANG THỰC ĐƠN (Đã sửa để nhận diện đúng loại từ Menu chuyển sang)
        public async Task<IActionResult> Index(int loai = 1)
        {
            var thucDon = await _context.DichVus
                .Where(d => d.LoaiDichVu == loai)
                .ToListAsync();

            ViewBag.LoaiHienTai = loai;
            return View(thucDon);
        }

        // 2. TRANG DỊCH VỤ THUÊ
        public async Task<IActionResult> ThueDo()
        {
            var doThue = await _context.DichVus
                .Where(d => d.LoaiDichVu == 2)
                .ToListAsync();

            ViewBag.LoaiHienTai = 2;
            return View("Index", doThue);
        }

        // 3. GIAO DIỆN FORM THÊM MỚI 
        public IActionResult Create(int loai)
        {
            var model = new DichVu { LoaiDichVu = loai };
            return View(model);
        }

        // 4. XỬ LÝ LƯU THÊM MỚI VÀO DATABASE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm thành công sản phẩm/dịch vụ mới!";

                if (dichVu.LoaiDichVu == 2) return RedirectToAction(nameof(ThueDo));
                return RedirectToAction(nameof(Index), new { loai = dichVu.LoaiDichVu });
            }
            return View(dichVu);
        }

        // 5. XỬ LÝ XÓA DỊCH VỤ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int loai)
        {
            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu != null)
            {
                _context.DichVus.Remove(dichVu);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa sản phẩm/dịch vụ thành công!";
            }

            if (loai == 2) return RedirectToAction(nameof(ThueDo));
            return RedirectToAction(nameof(Index), new { loai = loai });
        }

        // 6. GIAO DIỆN FORM CHỈNH SỬA
        public async Task<IActionResult> Edit(int? id, int loai)
        {
            if (id == null) return NotFound();

            var dichVu = await _context.DichVus.FindAsync(id);
            if (dichVu == null) return NotFound();

            ViewBag.LoaiHienTai = loai;
            return View(dichVu);
        }

        // 7. XỬ LÝ LƯU CẬP NHẬT CHỈNH SỬA (Đã sửa lỗi đẻ nhánh bản sao dữ liệu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Entry(dichVu).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Cập nhật thông tin thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DichVus.Any(e => e.DichVuId == dichVu.DichVuId)) return NotFound();
                    else throw;
                }

                if (dichVu.LoaiDichVu == 2) return RedirectToAction(nameof(ThueDo));
                return RedirectToAction(nameof(Index), new { loai = dichVu.LoaiDichVu });
            }

            ViewBag.LoaiHienTai = dichVu.LoaiDichVu;
            return View(dichVu);
        }
    }
}