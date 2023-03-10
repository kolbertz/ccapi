using System;
using System.Collections.Generic;

namespace CCProductService.Data;

public partial class InternalProductString
{
    public Guid Id { get; set; }

    public string Language { get; set; }

    public string ShortName { get; set; }

    public string LongName { get; set; }

    public string Description { get; set; }

    public Guid ProductId { get; set; }

    public virtual InternalProduct Product { get; set; }
}
