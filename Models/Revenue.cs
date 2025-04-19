using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Revenue
{
    public int RevenueId { get; set; }

    public DateOnly? RevenueDate { get; set; }

    public int? Year { get; set; }

    public int? Month { get; set; }

    public int? PhongKhamId { get; set; }

    public int? ProductId { get; set; }

    public decimal? TotalRevenue { get; set; }

    public virtual PhongKham? PhongKham { get; set; }

    public virtual Product? Product { get; set; }
}
