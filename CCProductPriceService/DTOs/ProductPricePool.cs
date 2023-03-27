using CCApiLibrary.Models;
using CCProductPriceService.InternalData;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

namespace CCProductPriceService.DTOs
{
    public class ProductPricePool : ProductPricePoolBase
    {
        public Guid Id { get; set; }

        public ProductPricePool(InternalProductPricePool internalPool) : base(internalPool)
        {
            if (internalPool != null)
            {
                Id = internalPool.Id;
                //TODO: Change when multilanguage for ProcePool is available
                Name.Add(new MultilanguageText { Culture = "de-DE", Text = internalPool.Name });
                if (!string.IsNullOrEmpty(internalPool.Description))
                {
                    Description.Add(new MultilanguageText { Culture = "de-DE", Text = internalPool.Description });
                }
                ParentPoolId = internalPool.ParentProductPricePoolId;
                SystemSettingsId = internalPool.SystemSettingsId;
                CreatedDate = internalPool.CreatedDate;
                CreatedUser = internalPool.CreatedUser;
                LastUpdatedDate = internalPool.LastUpdatedDate;
                LastUpdatedUser = internalPool.LastUpdatedUser;
                CurrencyId = internalPool.CurrencyId;
            }
        }

        public DateTimeOffset CreatedDate { get; set; }
        public Guid CreatedUser { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public Guid LastUpdatedUser { get; set; }

        public ProductPricePool(InternalProductPricePool internalProductPricePool, Guid sysId)
        {
            if (internalProductPricePool != null)
            {
                Id = internalProductPricePool.Id;
                
                Name.Add(new MultilanguageText { Culture = "de-DE", Text = internalProductPricePool.Name });
                if (!string.IsNullOrEmpty(internalProductPricePool.Description))
                {
                    Description.Add(new MultilanguageText { Culture = "de-DE", Text = internalProductPricePool.Description });
                }
                SystemSettingsId = sysId;
                CreatedDate = internalProductPricePool.CreatedDate;
                CreatedUser = internalProductPricePool.CreatedUser;
                LastUpdatedDate = internalProductPricePool.LastUpdatedDate;
                LastUpdatedUser = internalProductPricePool.LastUpdatedUser;
                CurrencyId = internalProductPricePool.CurrencyId;
            }
        }

        public ProductPricePool() { }

        //public ProductPricePool(InternalProductPricePool internalPool) : base(internalPool)
        //{
        //    Id = internalPool.Id;
        //}
    }
}
