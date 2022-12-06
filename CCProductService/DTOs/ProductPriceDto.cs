namespace CCProductService.DTOs
{
    public class ProductPriceDto
    {
        public string PoolName { get; set; }
        public IEnumerable<PriceListPrice> PriceListPrice { get; set; }
    }
}
