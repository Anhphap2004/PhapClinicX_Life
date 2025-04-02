using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class ServiceCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ServicePackage> ServicePackages { get; set; } = new List<ServicePackage>();
}
