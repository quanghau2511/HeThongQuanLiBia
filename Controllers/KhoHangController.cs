using Microsoft.AspNetCore.Mvc;

namespace HeThongQuanLiBia.Controllers
{
    public class KhoHangController : Controller
    {
        // Trang tổng quan chính của Kho Hàng & Danh Mục
        public IActionResult Index()
        {
            return View();
        }
    }
}