using CCApiLibrary.Models;
using System.ComponentModel.DataAnnotations;

namespace CCProductPriceService.DTOs
{
    public class ProductPricePoolBase
    {
        public Guid Id { get; set; }

        [Required]
        public List<MultilanguageText> Name { get; set; }

        public List<MultilanguageText> Description { get; set; }

        public Guid? ParentPoolId { get; set; }

        [Required]
        public Guid SystemSettingsId { get; set; }

        public ProductPricePoolBase() { 
            Name = new List<MultilanguageText>();
            Description= new List<MultilanguageText>();
        }
    }
}
