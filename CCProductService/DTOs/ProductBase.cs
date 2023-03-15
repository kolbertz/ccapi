using CCApiLibrary.Models;
using CCProductService.Data;
using CCProductService.DTOs.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CCProductService.DTOs
{

    public class Product : ProductBase
    {
        public Guid Id { get; set; }

        public Product() { }

        public Product (InternalProduct internalProduct) 
        { 
            Id= internalProduct.Id;
        }

    }
    public class ProductBase 
    {      

        public Guid ProductPoolId { get; set; }

        [Required]
        public int Key { get; set; }

        public List<string> Barcodes { get; set; }

        [Required]
        public List<MultilanguageText> ShortNames { get; set; }

        [Required]
        public List<MultilanguageText> LongNames { get; set; }
        
        public List<MultilanguageText> Descriptions { get; set; }

        [Required]
        public ProductType ProductType { get; set; }

        public IEnumerable<ImageUrl> ImageUrls { get; set; }

        public Balance Balance { get; set; }

        public bool IsBlocked { get; set; }

        public bool IsDeleted { get; set; }
  

        public ProductBase() { }

        public ProductBase(InternalProduct product)
        {
            if (product != null)
            {
                
                ProductPoolId = product.ProductPoolId;
                Key = product.ProductKey;
                Barcodes = product.ProductBarcodes?.Select(x => x.Barcode).ToList();
                ShortNames = new List<MultilanguageText>();
                LongNames = new List<MultilanguageText>();
                Descriptions = new List<MultilanguageText>();
                Balance = product.Balance ? new Balance
                {
                    PriceUnit = product.BalancePriceUnit,
                    PriceUnitValue = product.BalancePriceUnitValue,
                    Tare = product.BalanceTare
                } : null;
                IsBlocked = product.IsBlocked;
            }
        }

        public void SetMultilanguageText(InternalProductString productString)
        {
            ShortNames.Add(new MultilanguageText { Culture = productString.Language, Text = productString.ShortName });
            LongNames.Add(new MultilanguageText { Culture = productString.Language, Text = productString.LongName });
            Descriptions.Add(new MultilanguageText { Culture = productString.Language, Text = productString.Description });
        }
    }
}
