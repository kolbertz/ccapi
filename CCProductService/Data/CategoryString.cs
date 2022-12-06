using System;
using System.Collections.Generic;

namespace CCProductService.Data;

public partial class CategoryString
{
    public Guid Id { get; set; }

    public string Culture { get; set; }

    public string CategoryName { get; set; }

    public string Description { get; set; }

    public string Comment { get; set; }

    public Guid CategoryId { get; set; }

    public virtual Category Category { get; set; }
}
