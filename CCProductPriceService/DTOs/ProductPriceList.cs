﻿using CCApiLibrary.Models;
using CCProductPriceService.InternalData;

namespace CCProductPriceService.DTOs
{
    public class ProductPriceList : ProductPriceListBase
    {
        public ProductPriceList() { }

        public ProductPriceList(InternalProductPriceList internalProductPriceList)

        {
            if (internalProductPriceList != null) 
            
            { 
                Id= internalProductPriceList.Id;
                Name.Add(new MultilanguageText { Culture = "de-DE", Text = internalProductPriceList.Name });
                SystemSettingsId= internalProductPriceList.SystemSettingsId;
               
            }
        }        

    }
}
