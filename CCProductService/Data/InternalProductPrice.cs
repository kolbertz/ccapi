using CCProductService.DTOs;

namespace CCProductService.Data
{
    public class InternalProductPrice
    {
        public InternalProductPrice(List<ProductPriceBase> productPriceBases)
        {

            ProductId = productPriceBases[0].ProductId;
            Guid PoolId = productPriceBases[0].PricePoolId;
            Guid ProductPriceListId = productPriceBases[0].PriceListId;
            decimal ManuPrice = productPriceBases[0].Price;

        }

        public InternalProductPrice()
        {
        }
        public Guid ProductPriceId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public decimal Value { get; set; }
        public string CurrencySymbol { get; set; }
        public Guid ProductId { get; internal set; }

    }
}
