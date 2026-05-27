using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BansController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var danhSachBan = await _context.Bans.ToListAsync();
            return View(danhSachBan);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ban = await _context.Bans.FirstOrDefaultAsync(m => m.BanId == id);
            if (ban == null) return NotFound();

            return View(ban);
        }

        public IActionResult Create()
        {
            return View(new Bans
            {
                TrangThai = 0,
                TinhTrang = "Tốt",
                GiaTheoGio = 0m
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenBan,GiaTheoGio,TinhTrang,TrangThai")] Bans ban)
        {
            if (ModelState.IsValid)
            {
                _context.Bans.Add(ban);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm bàn bida mới thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(ban);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ban = await _context.Bans.FindAsync(id);
            if (ban == null) return NotFound();
            return View(ban);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BanId,TenBan,GiaTheoGio,TinhTrang,TrangThai")] Bans ban)
        {
            if (id != ban.BanId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ban);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin bàn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BanExists(ban.BanId)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ban);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ban = await _context.Bans.FirstOrDefaultAsync(m => m.BanId == id);
            if (ban == null) return NotFound();

            return View(ban);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ban = await _context.Bans.FindAsync(id);
            if (ban != null)
            {
                _context.Bans.Remove(ban);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã xóa bàn bida thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BanExists(int id)
        {
            return _context.Bans.Any(e => e.BanId == id);
        }
    }
}