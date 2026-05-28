using System.ComponentModel.DataAnnotations;

namespace HeThongQuanLiBia.Models;

public class KhachHang
{
    [Key]
    public int KhachHangId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập họ tên")]
    [Display(Name = "Họ tên")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
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
    [Display(Name = "Mật khẩu")]
    [DataType(DataType.Password)]
    [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
    public string MatKhau { get; set; } = string.Empty;

    [Display(Name = "Ngày tạo")]
    public DateTime NgayTao { get; set; } = DateTime.Now;

    public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
}
