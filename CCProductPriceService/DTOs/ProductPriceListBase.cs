using CCApiLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace CCProductPriceService.DTOs
{
    public class ProductPriceListBase
    {
        public Guid Id { get; set; }

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
    }
}
