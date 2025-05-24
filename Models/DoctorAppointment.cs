using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class DoctorAppointment
{
    public int AppointmentId { get; set; }

    public int? UserId { get; set; }

    public int DoctorId { get; set; }

    public DateTime? DateTime { get; set; }

    public bool Status { get; set; }

    public string Fullname { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual DoctorProfile? Doctor { get; set; }

    public virtual User? User { get; set; }
}