using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class DoanhThuTong
{
    public int DoanhThuTongId { get; set; }

    public int? Thang { get; set; }

    public int? Nam { get; set; }

    public decimal TongDoanhThu { get; set; }
}
