using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class DoanhThuPhongKham
{
    public int DoanhThuId { get; set; }

    public int? PhongKhamId { get; set; }

    public int? Thang { get; set; }

    public int? Nam { get; set; }

    public decimal DoanhThu { get; set; }

    public virtual PhongKham? PhongKham { get; set; }
}
