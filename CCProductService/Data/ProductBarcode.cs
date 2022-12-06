using System;
using System.Collections.Generic;

namespace CCProductService.Data;

public partial class ProductBarcode
{
    public Guid Id { get; set; }

    public string Barcode { get; set; }

    public bool Refund { get; set; }

    public Guid ProductId { get; set; }

    public virtual Product Product { get; set; }
}
