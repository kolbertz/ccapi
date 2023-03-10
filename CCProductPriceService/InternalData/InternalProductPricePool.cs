namespace CCProductPriceService.InternalData
{
    public class InternalProductPricePool
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid CreatedUser { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public Guid LastUpdatedUser { get; set; }
        public Guid? ParentProductPricePoolId { get; set; }
        public Guid? CurrencyId { get; set; }
        public Guid SystemSettingsId { get; set; }
    }
}
