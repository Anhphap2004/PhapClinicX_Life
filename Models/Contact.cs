using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public int? UserId { get; set; }

    public string? Message { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
