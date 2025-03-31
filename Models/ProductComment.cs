using System;
using System.Collections.Generic;

namespace PhapClinicX.Models;

public partial class ProductComment
{
    public int CommentId { get; set; }

    public int? ProductId { get; set; }

    public int? UserId { get; set; }

    public string? Comment { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Image { get; set; }

    public int? Star { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
