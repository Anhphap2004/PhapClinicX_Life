using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class KhoaKham
{
    public int KhoaKhamId { get; set; }

    public string TenKhoa { get; set; } = null!;

    public string? MoTa { get; set; }

    public int? PhongKhamId { get; set; }

    public int? DoctorId { get; set; }

    public virtual PhongKham? PhongKham { get; set; }
}
