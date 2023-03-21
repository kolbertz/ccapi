﻿using CCApiLibrary.Models;
using CCProductService.Data;

namespace CCProductService.DTOs
{
    public class ProductPrice
    {
        public Guid PricePoolId { get; set; }
        public IEnumerable<MultilanguageText> PricePoolNames { get; set; }
        public Guid PriceListId{ get; set; }
        public List<MultilanguageText> PriceListNames { get; set; }
        public Guid ProductPriceId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public decimal Price { get; set; }
        public string CurrencyValue { get; set; }

        public ProductPrice() { }

        public ProductPrice(InternalProductPricePool internalPricePool, InternalProductPriceList internalProductPriceList, InternalProductPrice internalProductPrice)
        {
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
            ProductPriceId = internalProductPrice.ProductPriceId;
            StartDate = internalProductPrice.StartDate;
            Price = internalProductPrice.Value;
            CurrencyValue = internalProductPrice.CurrencySymbol;
        }
    }
}