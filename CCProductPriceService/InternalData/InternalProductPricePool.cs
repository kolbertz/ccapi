using CCProductPriceService.DTOs;
using System.ComponentModel.DataAnnotations;

namespace CCProductPriceService.InternalData
{
    public partial class InternalProductPricePool
    {
        [Key]
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

        public InternalProductPricePool(ProductPricePool pricePool ) 
        {
            MergeProductPricePool(pricePool);
        }

        public InternalProductPricePool() { }

        public void MergeProductPricePool(ProductPricePool pricePool ) 
        {         
            Id= pricePool.Id;
            Name = pricePool.Name;
            Description = pricePool.Description;
            ParentProductPricePoolId = pricePool.ParentPoolId;
            CurrencyId = pricePool.CurrencyId;
            SystemSettingsId= pricePool.SystemSettingsId;
        }
    }
}
