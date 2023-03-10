namespace CCProductPriceService.InternalData
{
    public class InternalProductPrice
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductPricePoolId { get; set; }
        public Guid ProductPriceListId { get; set; }
        public int ManualPrice { get; set; }
    }
}
