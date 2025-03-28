using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Discount
{
    public int DiscountId { get; set; }

    public string DiscountName { get; set; } = null!;

    public decimal DiscountPercent { get; set; }

    public string? Image { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }

    public string? Detail { get; set; }

    public string? Code { get; set; }
}
