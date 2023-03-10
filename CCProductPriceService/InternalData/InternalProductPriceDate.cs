namespace CCProductPriceService.InternalData
{
    public class InternalProductPriceDate
    {
        public Guid Id { get; set; }
        public Guid ProductPriceId { get; set; }
        public DateTimeOffset StartDate { get; set; }
        public decimal? Value { get; set; }
    }
}
