using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class ClinicAppointment
{
    public int AppointmentId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime? DateTime { get; set; }

    public bool Status { get; set; }

    public int PhongKhamId { get; set; }

    public virtual PhongKham? PhongKham { get; set; }
}