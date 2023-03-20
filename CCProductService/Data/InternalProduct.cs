using CCProductService.DTOs;
using System;
using System.Collections.Generic;

namespace CCProductService.Data;

public partial class InternalProduct
{
    public Guid Id { get; set; }

    public int ProductKey { get; set; }

    public decimal Standardprice { get; set; }

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

    public Guid? ProductPoolId { get; set; }

    public string Image { get; set; }

    public byte ProductType { get; set; }

    public virtual ICollection<InternalProductBarcode> ProductBarcodes { get; } = new List<InternalProductBarcode>();

    public virtual ICollection<InternalProductString> ProductStrings { get; } = new List<InternalProductString>();

    public InternalProduct() { }

    public InternalProduct(Product product)
    {

        if (product != null)
        {
            Id = product.Id;
            ProductKey = product.Key.Value;
            List<string> cultures = product.ShortNames.Select(sn => sn.Culture).ToList();
            if (cultures != null && cultures.Count > 0)
            {
                foreach (string culture in cultures)
                {
                    ProductStrings.Add(new InternalProductString
                    {
                        ProductId = product.Id,
                        Language = culture,
                        ShortName = product.ShortNames?.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                        LongName = product.LongNames?.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                        Description = product.Descriptions?.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault()
                    });
                }
            }

        }
    }
}
