using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class InvoiceDetail
{
    public int DetailId { get; set; }

    public int? InvoiceId { get; set; }

    public int? ServiceId { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public int? ProductId { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual Product? Product { get; set; }
}
