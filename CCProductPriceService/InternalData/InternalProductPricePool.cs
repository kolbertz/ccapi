using CCProductPriceService.DTOs;

namespace CCProductPriceService.InternalData
{
    public partial class InternalProductPricePool
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

        public InternalProductPricePool(ProductPricePoolBase pricePoolBase ) 
        {
            MergeProductPricePool(pricePoolBase);
        }

        public void MergeProductPricePool(ProductPricePoolBase pricePoolBase ) 
        {            
            //Name = pricePoolBase.Name;
            //Description = pricePoolBase.Description;
            ParentProductPricePoolId = pricePoolBase.ParentPoolId;
            CurrencyId = pricePoolBase.CurrencyId;
            SystemSettingsId= pricePoolBase.SystemSettingsId;
        }
    }
}
