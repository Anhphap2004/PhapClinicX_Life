using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Faq
{
    public int Id { get; set; }

    public string Question { get; set; } = null!;

    public string? Answer { get; set; }

    public DateTime? CreatedAt { get; set; }
}
