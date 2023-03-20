using CCApiLibrary.Models;
using CCProductPriceService.InternalData;
using System.ComponentModel.DataAnnotations;

namespace CCProductPriceService.DTOs
{
    public class ProductPricePoolBase
    {
        

        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }

        public Guid? ParentPoolId { get; set; }

        public Guid? CurrencyId { get; set; }

        [Required]
        public Guid SystemSettingsId { get; set; }

        public ProductPricePoolBase() { }

        public ProductPricePoolBase(InternalProductPricePool internalProductPricePool)
        {
            if (internalProductPricePool != null)
            {                
                Name = internalProductPricePool.Name;
                Description = internalProductPricePool.Description;
                ParentPoolId = internalProductPricePool.ParentProductPricePoolId;
                CurrencyId = internalProductPricePool.CurrencyId;
                SystemSettingsId = internalProductPricePool.SystemSettingsId;
            }
          
        }
    }
}
