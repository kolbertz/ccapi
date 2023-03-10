using CCApiLibrary.Models;
using CCProductService.Data;
using CCProductService.DTOs;
using CCProductService.DTOs.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CCProductService.Helper
{
    public class ProductHelper
    {
        public static void ParseProductToDto(Product product, ProductDto productDto)
        {
            if (product != null)
            {
                if (productDto == null)
                {
                    productDto = new ProductDto();
                }
                productDto.Id = product.Id;
                productDto.ProductPoolId = product.ProductPoolId;
                productDto.Key = product.ProductKey;
                productDto.Barcodes = product.ProductBarcodes?.Select(x => x.Barcode).ToList();
                productDto.ShortNames = product.ProductStrings?.Select(x => new MultilanguageText { Culture = x.Language, Text = x.ShortName }).ToList();
                productDto.LongNames = product.ProductStrings?.Select(x => new MultilanguageText { Culture = x.Language, Text = x.LongName }).ToList();
                productDto.Descriptions = product.ProductStrings?.Select(x => new MultilanguageText { Culture = x.Language, Text = x.Description }).ToList();
                productDto.Balance = product.Balance ? new Balance
                {
                    PriceUnit = product.BalancePriceUnit,
                    PriceUnitValue = product.BalancePriceUnitValue,
                    Tare = product.BalanceTare
                } : null;
                productDto.IsBlocked = product.IsBlocked;
            }

        }

        public static void ParseDtoToProduct(ProductDto productDto, Product product)
        {
            if (productDto != null)
            {
                if (product == null)
                {
                    product = new Product();
                }
                product.Id = productDto.Id;
                product.ProductPoolId = productDto.ProductPoolId;
                product.ProductKey = productDto.Key;
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
                        product.ProductBarcodes.Add(new ProductBarcode { Barcode = barcode, ProductId = product.Id });
                    }
                }
                List<string> cultures = productDto.ShortNames.Select(sn => sn.Culture).ToList();
                cultures.AddRange(productDto.LongNames.Where(ln => !cultures.Contains(ln.Culture)).Select(ln => ln.Culture));
                cultures.AddRange(productDto.Descriptions.Where(ld => !cultures.Contains(ld.Culture)).Select(ld => ld.Culture));
                if (cultures != null && cultures.Count > 0)
                {
                    product.ProductStrings.Clear();
                    foreach (string culture in cultures)
                    {
                        product.ProductStrings.Add(new ProductString
                        {
                            ProductId = product.Id,
                            Language = culture,
                            ShortName = productDto.ShortNames.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                            LongName = productDto.LongNames.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault(),
                            Description = productDto.Descriptions.Where(x => x.Culture == culture).Select(x => x.Text).FirstOrDefault()
                        });
                    }
                }
            }
        }
    }
}
