﻿using System;
using System.Collections.Generic;

namespace CCProductService.Data;

public partial class InternalProductBarcode
{
    public Guid Id { get; set; }

    public string Barcode { get; set; }

    public bool Refund { get; set; }

    public Guid ProductId { get; set; }

    public virtual InternalProduct Product { get; set; }
}
