using CCApiLibrary.Models;
using CCProductPriceService.InternalData;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace CCProductPriceService.DTOs
{
  
    public class ProductPriceListBase
    {

        [Required]
        public List<MultilanguageText> Name { get; set; }
        [Required]
        public int? Key { get; set; }

        public int Priority { get; set; }

        [Required]
        public Guid? SystemSettingsId { get; set; }

        public ProductPriceListBase()
        {
            Name = new List<MultilanguageText>();
        }

        public ProductPriceListBase(InternalProductPriceList internalProductPriceList) : base()
        {
            if (internalProductPriceList != null)
            {
                Key = internalProductPriceList.Key;
              
                Priority = internalProductPriceList.Priority;
                SystemSettingsId = internalProductPriceList.SystemSettingsId;
            }
        }

    }
}
