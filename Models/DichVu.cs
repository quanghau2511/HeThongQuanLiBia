using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLiBia.Models
{
    public class DichVu
    {
        [Key]
        public int DichVuId { get; set; }

        [Required(ErrorMessage = "Tên dịch vụ không được để trống")]
        public string TenDichVu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Đơn vị tính không được để trống")]
        public string DonViTinh { get; set; } = string.Empty; // Lon, Đĩa, Gói, Cái...

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue)]
        public decimal Gia { get; set; }

        public int SoLuongTon { get; set; }

        /// <summary>
        /// Phân loại dịch vụ:
        /// 1: Nước uống / Thuốc lá (Thực đơn)
        /// 2: Phụ kiện cho thuê (Cơ, Găng tay, Lơ)
        /// </summary>
        [Required]
        public int LoaiDichVu { get; set; }

        // Mối quan hệ với bảng chi tiết hóa đơn
        public ICollection<ChiTietDichVu>? ChiTietDichVus { get; set; }
    }

}