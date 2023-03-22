using CCApiLibrary.Models;
using CCProductPriceService.DTOs;

namespace CCProductPriceService.InternalData
{
    public partial class InternalProductPriceList
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Key { get; set; }

        public int Priority { get; set; }

        public Guid SystemSettingsId { get; set; }      

        public InternalProductPriceList(ProductPriceListBase productPriceListBase) 
        {
            MergeProductPriceListBase(productPriceListBase);
        }

        public void MergeProductPriceListBase(ProductPriceListBase priceList)
        {
            
            if (priceList.Name != null && priceList.Name.Count > 0)
            {
                Name = priceList.Name.First().Text;
            }
            Key = priceList.Key.Value;
            Priority = priceList.Priority;
            SystemSettingsId = priceList.SystemSettingsId.Value;
        }

        public InternalProductPriceList(ProductPriceList productPriceList):base() 
        {
            MergeProductPriceList(productPriceList);
        }

        public void MergeProductPriceList(ProductPriceList priceList)
        {
            Id= priceList.Id;
            if (priceList.Name != null && priceList.Name.Count > 0)
            {
                Name = priceList.Name.First().Text;
            }
            Key = priceList.Key.Value;
            Priority = priceList.Priority;
            SystemSettingsId = priceList.SystemSettingsId.Value;
        }
        public InternalProductPriceList() 
        {
           
        }
    }
}
