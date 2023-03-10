﻿using CCApiLibrary.Models;
using CCProductPriceService.InternalData;

namespace CCProductPriceService.DTOs
{
    public class ProductPricePool : ProductPricePoolBase
    {
        public ProductPricePool() { }

        public ProductPricePool(InternalProductPricePool internalPool)
        {
            if (internalPool != null)
            {
                Id = internalPool.Id;
                Name.Add(new MultilanguageText { Culture = "de-DE", Text = internalPool.Name });
                if (!string.IsNullOrEmpty(internalPool.Description))
                {
                    Description.Add(new MultilanguageText { Culture = "de-DE", Text = internalPool.Description });
                }
                ParentPoolId = internalPool.ParentProductPricePoolId;
                SystemSettingsId = internalPool.SystemSettingsId;
                CreatedDate = internalPool.CreatedDate;
                CreatedUser= internalPool.CreatedUser;
                LastUpdatedDate= internalPool.LastUpdatedDate;
                LastUpdatedUser= internalPool.LastUpdatedUser;
                CurrencyId= internalPool.CurrencyId;
            }
        }

        public DateTimeOffset CreatedDate { get; set; }
        public Guid CreatedUser { get; set; }
        public DateTimeOffset LastUpdatedDate { get; set; }
        public Guid LastUpdatedUser { get; set; }
        public Guid? CurrencyId { get; set; }
    }
}