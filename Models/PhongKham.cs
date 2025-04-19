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

    public string? Image { get; set; }

    public bool Isactive { get; set; }

    public string? Introduce { get; set; }

    public virtual ICollection<BranchProduct> BranchProducts { get; set; } = new List<BranchProduct>();

    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();

    public virtual ICollection<ClinicAppointment> ClinicAppointments { get; set; } = new List<ClinicAppointment>();

    public virtual ICollection<DoctorProfile> DoctorProfiles { get; set; } = new List<DoctorProfile>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    public virtual ICollection<KhoaKham> KhoaKhams { get; set; } = new List<KhoaKham>();

    public virtual ICollection<Revenue> Revenues { get; set; } = new List<Revenue>();
}
