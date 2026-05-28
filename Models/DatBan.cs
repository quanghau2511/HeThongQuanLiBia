using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HeThongQuanLiBia.Models;

public class DatBan
{
    [Key]
    public int DatBanId { get; set; }

    [Required]
    public int KhachHangId { get; set; }

    [ForeignKey(nameof(KhachHangId))]
    public virtual KhachHang? KhachHang { get; set; }

    [Required]
    public int BanId { get; set; }

    [ForeignKey(nameof(BanId))]
    public virtual Bans? Ban { get; set; }

    [Required]
    public DateTime NgayDat { get; set; }

    [Required]
    public TimeOnly GioBatDau { get; set; }

    [Required]
    public TimeOnly GioKetThuc { get; set; }

    [Display(Name = "Ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
    public string? GhiChu { get; set; }

    [Display(Name = "Trạng thái")]
    public int TrangThai { get; set; } = 0;

    public virtual ICollection<DatBanDichVu> DatBanDichVus { get; set; } = new List<DatBanDichVu>();
}
