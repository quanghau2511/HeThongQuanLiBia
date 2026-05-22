using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLiBia.Models
{
    public class ChiTietDichVu
    {
        [Key]
        public int ChiTietId { get; set; }

        [Required]
        public int HoaDonId { get; set; }
        [ForeignKey("HoaDonId")]
        public virtual HoaDon? HoaDon { get; set; }

        [Required]
        public int DichVuId { get; set; }
        [ForeignKey("DichVuId")]
        public virtual DichVu? DichVu { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DonGiaTaiThoiDiemBan { get; set; }
    }
}