using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class ClinicAppointment
{
    public int AppointmentId { get; set; }

    public int? UserId { get; set; }

    public int? ClinicId { get; set; }

    public DateTime? DateTime { get; set; }

    public string? Status { get; set; }

    public virtual User? User { get; set; }
}
