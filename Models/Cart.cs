using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Cart
{
    public int CartId { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int? Quantity { get; set; }

    public bool? IsCheckedOut { get; set; }

    public int? PhongKhamId { get; set; }

    public virtual PhongKham? PhongKham { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
