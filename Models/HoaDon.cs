using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm thư viện này

namespace HeThongQuanLiBia.Models;

public class HoaDon
{
    [Key]
    public int HoaDonId { get; set; }
    public int BanId { get; set; }
    public DateTime GioVao { get; set; }
    public DateTime? GioRa { get; set; }

    [Column(TypeName = "decimal(18,2)")] // Định rõ kiểu dữ liệu để hết cảnh báo
    public decimal TongTienDichVu { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTienGio { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal TongTien { get; set; }

    public bool DaThanhToan { get; set; }

    public virtual Bans? Ban { get; set; }
    public virtual ICollection<ChiTietDichVu> ChiTietDichVus { get; set; } = new List<ChiTietDichVu>();
    // The database columns are GioVao / GioRa. Provide convenience (non-mapped)
    // properties ThoiGianVao/ThoiGianRa used throughout the app so views/controllers
    // can use those names without changing the database schema.
    [NotMapped]
    public DateTime ThoiGianVao
    {
        get => GioVao;
        set => GioVao = value;
    }

    [NotMapped]
    public DateTime? ThoiGianRa
    {
        get => GioRa;
        set => GioRa = value;
    }
}