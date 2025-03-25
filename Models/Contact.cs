using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public string Fullname { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int Phone { get; set; }

    public string Email { get; set; } = null!;

    public bool Status { get; set; }
}
