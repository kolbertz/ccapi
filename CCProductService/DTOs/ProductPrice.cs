using CCApiLibrary.Models;
using CCProductService.Data;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace CCProductService.DTOs
{
    public class ProductPriceBase
    {
        public Guid ProductId { get; set; }

        public Guid PricePoolId { get; set; }

        public Guid PriceListId { get; set; }
        
        public DateTimeOffset? StartDate { get; set; }

        public decimal Price { get; set; }
       
    }

    public class ProductPriceDate
    {
        public Guid PriceId { get; set; }
        public decimal Value { get; set; }
        public DateTimeOffset Date { get; set; }
    }

    public class ProductPrice : ProductPriceBase
    {

        public IEnumerable<MultilanguageText> PricePoolNames { get; set; }
        public List<MultilanguageText> PriceListNames { get; set; }
        public string CurrencyValue { get; set; }

        public ProductPrice() { }

        public ProductPrice(InternalProductPricePool internalPricePool, InternalProductPriceList internalProductPriceList, InternalProductPrice internalProductPrice)
        {
            ProductId = internalProductPrice.ProductId;
            PricePoolId = internalPricePool.ProductPricePoolId;
            PricePoolNames = new List<MultilanguageText>
            {
                new MultilanguageText("de-DE", internalPricePool.PricePoolName)
            };
            PriceListId = internalProductPriceList.ProductPriceListId;
            PriceListNames = new List<MultilanguageText>
            {
                new MultilanguageText("de-DE", internalProductPriceList.PriceListName)
            };          
            
            StartDate = internalProductPrice.StartDate;
            Price = internalProductPrice.Value;
            CurrencyValue = internalProductPrice.CurrencySymbol;
        }
    }
}
