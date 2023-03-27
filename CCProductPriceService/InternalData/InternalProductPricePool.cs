using CCProductPriceService.DTOs;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
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

        public InternalProductPricePool(ProductPricePoolBase pricePoolBase )
        {

            if (pricePoolBase.Name != null && pricePoolBase.Name.Count > 0)
            {
                Name = pricePoolBase.Name.First().Text;
            }
            if (pricePoolBase.Description != null && pricePoolBase.Description.Count > 0)
            {
                Description = pricePoolBase.Description.First().Text;
            }
            ParentProductPricePoolId = pricePoolBase.ParentPoolId;
            CurrencyId = pricePoolBase.CurrencyId;
            SystemSettingsId = pricePoolBase.SystemSettingsId.Value;
        }

        public InternalProductPricePool() { }

        public InternalProductPricePool(ProductPricePool pricePool) : base()
        {
            MergeProductPricePool(pricePool);
        }
        public void MergeProductPricePool(ProductPricePool pricePool ) 
        {         
            Id= pricePool.Id;
            if (pricePool.Name != null && pricePool.Name.Count > 0)
            {
                Name = pricePool.Name.First().Text;
            }
            if (pricePool.Description != null && pricePool.Description.Count > 0)
            {
                Description = pricePool.Description.First().Text;
            }
            ParentProductPricePoolId = pricePool.ParentPoolId;
            CurrencyId = pricePool.CurrencyId;
            SystemSettingsId= pricePool.SystemSettingsId.Value;
        }

       
    }
}
