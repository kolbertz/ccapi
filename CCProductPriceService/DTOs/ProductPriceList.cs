using CCApiLibrary.Models;
using CCProductPriceService.InternalData;

namespace CCProductPriceService.DTOs
{
    public class ProductPriceList : ProductPriceListBase
    {
        public Guid Id { get; set; }

        public ProductPriceList(InternalProductPriceList internalProductPriceList) 

        {
            if (internalProductPriceList != null) 
            
            { 
                Id= internalProductPriceList.Id;
                Name.Add(new MultilanguageText { Culture = "de-DE", Text = internalProductPriceList.Name });
                SystemSettingsId= internalProductPriceList.SystemSettingsId;
               
            }
        } 
        public ProductPriceList(InternalProductPriceList internalProductPriceList, Guid sysId)  
        {
            if (internalProductPriceList != null)
            {
                Id = internalProductPriceList.Id;
                Key = internalProductPriceList.Key;
                Name.Add(new MultilanguageText { Culture = "de-DE", Text = internalProductPriceList.Name });
                Priority = internalProductPriceList.Priority;
                SystemSettingsId = internalProductPriceList.SystemSettingsId;
            }
        }


    }
}
