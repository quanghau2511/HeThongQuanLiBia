using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLiBia.Models;

public class KhachHangRegisterViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [Display(Name = "Họ tên")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
    [Display(Name = "Số điện thoại")]
    [RegularExpression(@"^(0|\+84)[0-9]{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
    public string SoDienThoai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
    [DataType(DataType.Password)]
    [Compare("MatKhau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    [Display(Name = "Xác nhận mật khẩu")]
    public string XacNhanMatKhau { get; set; } = string.Empty;
}

public class KhachHangLoginViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập email")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mật khẩu")]
    public string MatKhau { get; set; } = string.Empty;
}

public class DatBanBookingViewModel
{
    [Required(ErrorMessage = "Vui lòng chọn bàn")]
    [Display(Name = "Bàn bida")]
    public int BanId { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày chơi")]
    [Display(Name = "Ngày chơi")]
    [DataType(DataType.Date)]
    public DateTime NgayDat { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Vui lòng chọn giờ bắt đầu")]
    [Display(Name = "Giờ bắt đầu")]
    [DataType(DataType.Time)]
    public TimeOnly GioBatDau { get; set; } = TimeOnly.FromDateTime(DateTime.Now);

    [Required(ErrorMessage = "Vui lòng chọn giờ kết thúc")]
    [Display(Name = "Giờ kết thúc")]
    [DataType(DataType.Time)]
    public TimeOnly GioKetThuc { get; set; } = TimeOnly.FromDateTime(DateTime.Now.AddHours(2));

    [Display(Name = "Ghi chú")]
    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự")]
    public string? GhiChu { get; set; }

    public string SelectedServicesJson { get; set; } = "[]";
}

public class SelectedServiceItem
{
    public int DichVuId { get; set; }

    public int SoLuong { get; set; }
}
