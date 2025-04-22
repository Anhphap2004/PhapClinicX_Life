using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int? UserId { get; set; }

    public decimal? TotalAmount { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int PhongKhamId { get; set; }

    public string? InvoiceType { get; set; }

    public string? Method { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? DiscountCode { get; set; }
    public int? DiscountId { get; set; }
    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual PhongKham PhongKham { get; set; } = null!;

    public virtual User? User { get; set; }
}
