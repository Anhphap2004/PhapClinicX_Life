using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class ServicePackage
{
    public int PackageId { get; set; }

    public string? PackageName { get; set; }

    public decimal? Price { get; set; }

    public bool IsActive { get; set; }

    public string? Image { get; set; }

    public string? Title { get; set; }

    public DateTime Date { get; set; }

    public string? Detail { get; set; }
}
