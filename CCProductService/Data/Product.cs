using System;
using System.Collections.Generic;

namespace CCProductService.Data;

public partial class Product
{
    public Guid Id { get; set; }

    public int ProductKey { get; set; }

    public bool IsBlocked { get; set; }

    public string Comment { get; set; }

    public int? ClientBlacklistType { get; set; }

    public string ClientBlacklist { get; set; }

    public int? MinimumAge { get; set; }

    public bool Balance { get; set; }

    public int? BalanceTare { get; set; }

    public bool? BalanceTareBarcode { get; set; }

    public string BalancePriceUnit { get; set; }

    public int? BalancePriceUnitValue { get; set; }

    public DateTimeOffset CreatedDate { get; set; }

    public Guid CreatedUser { get; set; }

    public DateTimeOffset LastUpdatedDate { get; set; }

    public Guid LastUpdatedUser { get; set; }

    public Guid ProductPoolId { get; set; }

    public string Image { get; set; }

    public byte ProductType { get; set; }

    public virtual ICollection<ProductBarcode> ProductBarcodes { get; } = new List<ProductBarcode>();

    public virtual ICollection<ProductString> ProductStrings { get; } = new List<ProductString>();

    public virtual ICollection<Category> Categories { get; } = new List<Category>();
}
