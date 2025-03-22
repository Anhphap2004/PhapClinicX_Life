using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class PhongKham
{
    public int PhongKhamId { get; set; }

    public string? TenPhongKham { get; set; }

    public string? DiaChi { get; set; }

    public string SoDienThoai { get; set; } = null!;

    public string? Email { get; set; }

    public DateOnly? NgayThanhLap { get; set; }

    public virtual ICollection<DoanhThuPhongKham> DoanhThuPhongKhams { get; set; } = new List<DoanhThuPhongKham>();

    public virtual ICollection<DoctorProfile> DoctorProfiles { get; set; } = new List<DoctorProfile>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<KhoaKham> KhoaKhams { get; set; } = new List<KhoaKham>();
}
