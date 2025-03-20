using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class BlogComment
{
    public int CommentId { get; set; }

    public int? BlogId { get; set; }

    public int? UserId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Blog? Blog { get; set; }

    public virtual User? User { get; set; }
}
