using HeThongQuanLiBia.Data;
using HeThongQuanLiBia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HeThongQuanLiBia.Controllers
{
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

        public IActionResult Create()
        {
            return View(new Bans());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Bans ban)
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
        public async Task<IActionResult> Edit(Bans ban)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Entry(ban).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật thông tin bàn thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Bans.Any(e => e.BanId == ban.BanId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ban);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
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
    }
}