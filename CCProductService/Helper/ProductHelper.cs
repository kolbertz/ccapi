using CCApiLibrary.Models;
using CCProductService.Data;
using CCProductService.DTOs;
using CCProductService.DTOs.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CCProductService.Helper
{
    public class ProductHelper
    {
        public static void ParseDtoToProduct(ProductBase productDto, InternalProduct product)
        {
            if (productDto != null)
            {
                if (product == null)
                {
                    product = new InternalProduct();
                }
                
                product.ProductPoolId = productDto.ProductPoolId;
                product.ProductKey = productDto.Key.Value;
                product.ProductType = (byte)productDto.ProductType;
                product.IsBlocked = productDto.IsBlocked;
                if (productDto.Balance != null)
                {
                    product.Balance = true;
                    product.BalancePriceUnit = productDto.Balance.PriceUnit;
                    product.BalancePriceUnitValue = productDto.Balance.PriceUnitValue;
                    product.BalanceTare = productDto.Balance.Tare;
                }
                if (productDto.Barcodes != null && productDto.Barcodes.Count() > 0)
                {
                    product.ProductBarcodes.Clear();
                    foreach (string barcode in productDto.Barcodes)
                    {
                        product.ProductBarcodes.Add(new InternalProductBarcode { Barcode = barcode, ProductId = product.Id });
                    }
                }
                List<string> cultures = new List<string>();
                if (productDto.ShortNames != null)
                {
                    cultures.AddRange(productDto.ShortNames.Select(sn => sn.Culture));
                }
                if (cultures.Count > 0)
                {
                    product.ProductStrings.Clear();
                    foreach (string culture in cultures)
                    {
                        product.ProductStrings.Add(new InternalProductString
                        {
                            ProductId = product.Id,
                            Language = culture,
                            ShortName = productDto.ShortNames?.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                            LongName = productDto.LongNames?.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                            Description = productDto.Descriptions?.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault()
                        });
                    }
                }
            }
        }
    }
}
