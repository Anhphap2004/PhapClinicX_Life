using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class DoctorProfile
{
    public int DoctorId { get; set; }

    public string? Specialization { get; set; }

    public int? Experience { get; set; }

    public string? Education { get; set; }

    public virtual User Doctor { get; set; } = null!;
}
