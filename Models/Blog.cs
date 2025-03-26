using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class Blog
{
    public int BlogId { get; set; }

    public int? CategoryId { get; set; }

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? Image { get; set; }

    public int? AuthorId { get; set; }

    public string? ExtraContent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public int? Viewcount { get; set; }

    public virtual User? Author { get; set; }

    public virtual ICollection<BlogComment> BlogComments { get; set; } = new List<BlogComment>();

    public virtual BlogCategory? Category { get; set; }
}
