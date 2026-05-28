using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLiBia.Models;

public class DatBanDichVu
{
    [Key]
    public int DatBanDichVuId { get; set; }

    [Required]
    public int DatBanId { get; set; }

    [ForeignKey(nameof(DatBanId))]
    public virtual DatBan? DatBan { get; set; }

    [Required]
    public int DichVuId { get; set; }

    [ForeignKey(nameof(DichVuId))]
    public virtual DichVu? DichVu { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
    public int SoLuong { get; set; } = 1;

    [Column(TypeName = "decimal(18,2)")]
    public decimal DonGiaTaiThoiDiemBan { get; set; }
}
