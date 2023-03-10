using CCApiLibrary.Models;
using CCProductService.Data;
using CCProductService.DTOs.Enums;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace CCProductService.DTOs
{
    public class ProductDto 
    {
        public Guid Id { get; set; }

        [Required]
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

        public ProductDto() { }

        public ProductDto(Product product)
        {
            if (product != null)
            {
                Id = product.Id;
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

        public void SetMultilanguageText(ProductString productString)
        {
            ShortNames.Add(new MultilanguageText { Culture = productString.Language, Text = productString.ShortName });
            LongNames.Add(new MultilanguageText { Culture = productString.Language, Text = productString.LongName });
            Descriptions.Add(new MultilanguageText { Culture = productString.Language, Text = productString.Description });
        }
    }
}
