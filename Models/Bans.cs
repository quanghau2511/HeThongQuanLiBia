using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLiBia.Models
{
    public class Bans
    {
        [Key]
        public int BanId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên bàn")]
        [Display(Name = "Tên Bàn")]
        public string TenBan { get; set; } = string.Empty;

        [Display(Name = "Trạng Thái Hoạt Động")]
        public int TrangThai { get; set; }

        [Display(Name = "Tình Trạng Vật Chất")]
        public string TinhTrang { get; set; } = "Tốt";

        [Display(Name = "Loại Bàn")]
        public int LoaiBan { get; set; }

        [Display(Name = "Giá theo giờ")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GiaTheoGio { get; set; }
        // Xóa bỏ dòng 'public decimal GiaGio { get; set; }' đi nhé!
    }
}