using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Menu
{
    public int MenuId { get; set; }

    public string? MenuName { get; set; }

    public string? Url { get; set; }

    public int? Position { get; set; }

    public int? ParentId { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Menu> InverseParent { get; set; } = new List<Menu>();

    public virtual Menu? Parent { get; set; }
}
