using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class BlogCategory
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public bool? IsActive { get; set; }

    public string? Alias { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();
}
