using CCApiLibrary.Models;
using CCProductPriceService.InternalData;
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

        public Guid? CurrencyId { get; set; }

        [Required]
        public Guid SystemSettingsId { get; set; }

        //public ProductPricePoolBase(InternalProductPricePool internalProductPricePool)
        //{
        //    Id = internalProductPricePool.Id;
        //    Name = new List<MultilanguageText>();
        //    Description = new List<MultilanguageText>();
        //    ParentPoolId = internalProductPricePool.ParentProductPricePoolId;
        //    CurrencyId = internalProductPricePool.CurrencyId;
        //    SystemSettingsId = internalProductPricePool.SystemSettingsId;
        //}
    }
}
