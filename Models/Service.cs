using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Service
{
    public int ServiceId { get; set; }

    public string? ServiceName { get; set; }

    public decimal? Price { get; set; }

    public bool? IsActive { get; set; }

    public string? Image { get; set; }

    public string? Detail { get; set; }
    public virtual ICollection<ServiceReview> ServiceReviews { get; set; } = new List<ServiceReview>();
}
