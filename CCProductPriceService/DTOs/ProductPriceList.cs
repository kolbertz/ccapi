using CCApiLibrary.Models;
using CCProductPriceService.InternalData;

namespace CCProductPriceService.DTOs
{
    public class ProductPriceList : ProductPriceListBase
    {
        public Guid Id { get; set; }

<<<<<<< HEAD
        public ProductPriceList(InternalProductPriceList internalProductPriceList) :base (internalProductPriceList)
=======
        public ProductPriceList() { }

        public ProductPriceList(InternalProductPriceList internalProductPriceList) 
>>>>>>> a3fe5e1716346170568099e984b56a0146f06c0d

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
        public ProductPriceList(InternalProductPriceList internalProductPriceList, Guid sysId)  
        {
            if (internalProductPriceList != null)
            {
                Id = internalProductPriceList.Id;
                Key = internalProductPriceList.Key;
                Name.Add(new MultilanguageText { Culture = "de-DE", Text = internalProductPriceList.Name });                
                Priority = internalProductPriceList.Priority;
                SystemSettingsId = sysId;
            }
        }

        public ProductPriceList() : base() { }
        
           
                    
    }
}
