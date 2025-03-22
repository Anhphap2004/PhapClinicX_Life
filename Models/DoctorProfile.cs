using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class DoctorProfile
{
    public int DoctorId { get; set; }

    public string? Specialization { get; set; }

    public int? Experience { get; set; }

    public string? Education { get; set; }

    public string? Image { get; set; }

    public bool Isactive { get; set; }

    public string? Fullname { get; set; }

    public string? Phone { get; set; }

    public int? PhongKhamId { get; set; }

    public virtual PhongKham? PhongKham { get; set; }
}
