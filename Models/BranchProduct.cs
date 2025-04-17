using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class BranchProduct
{
    public int ProductId { get; set; }

    public int PhongKhamId { get; set; }

    public int? Quantity { get; set; }

    public virtual PhongKham PhongKham { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
