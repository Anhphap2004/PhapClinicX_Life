using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class MedicalEquipment
{
    public int EquipmentId { get; set; }

    public string? EquipmentName { get; set; }

    public int? Quantity { get; set; }

    public bool? IsActive { get; set; }
}
