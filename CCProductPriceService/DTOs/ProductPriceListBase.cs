using CCApiLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace CCProductPriceService.DTOs
{
  
    public class ProductPriceListBase
    {

        [Required]
        public List<MultilanguageText> Name { get; set; }

        public int Key { get; set; }

        public int Priority { get; set; }

        [Required]
        public Guid SystemSettingsId { get; set; }

        public ProductPriceListBase()
        {
            Name = new List<MultilanguageText>();
        }
        public ProductPriceListBase(List<MultilanguageText> name, int key, int priority, Guid systemSettingsId)
        {
            Name = name;
            Key = key;
            Priority = priority;
            SystemSettingsId = systemSettingsId;
        }
    }
}
