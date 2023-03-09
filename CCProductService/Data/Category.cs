using System;
using System.Collections.Generic;

namespace CCProductService.Data;

public partial class Category
{
    public Guid Id { get; set; }

    public int CategoryKey { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public Guid CreatedUser { get; set; }

    public DateTimeOffset LastUpdatedDate { get; set; }

    public Guid LastUpdatedUser { get; set; }

    public Guid CategoryPoolId { get; set; }

    public virtual ICollection<CategoryString> CategoryStrings { get; } = new List<CategoryString>();

    public virtual ICollection<Product> Products { get; } = new List<Product>();

 
}
