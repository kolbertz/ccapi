namespace CCProductService.Data
{
    public class InternalProductPrice
    {
        public Guid ProductPriceId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public decimal Value { get; set; }
        public string CurrencySymbol { get; set; }
    }
}
