using CCApiLibrary.Models;
using CCProductPoolService.Data;
using System.ComponentModel.DataAnnotations;

namespace CCProductPoolService.Dtos
{
    public class ProductPoolBase
    {
       // public Guid Id { get; set; }
        [Required]
        public int? Key { get; set; }

        [Required]
        public List<MultilanguageText> Names { get; set; }

        public List<MultilanguageText> Descriptions { get; set; }

        public Guid? ParentProductPool { get; set; }

        [Required]
        public Guid? SystemSettingsId { get; set; }

        public ProductPoolBase() { }

        public ProductPoolBase(InternalProductPool internalProductPool)
        {
            if (internalProductPool != null)
            {                
                Key = internalProductPool.ProductPoolKey;
                Names = new List<MultilanguageText>
            {
                new MultilanguageText("de-DE", internalProductPool.Name)
            };
                Descriptions = new List<MultilanguageText>
            {
                new MultilanguageText ("de-DE", internalProductPool.Description)
            };
                ParentProductPool = internalProductPool.ParentProductPoolId;
                SystemSettingsId = internalProductPool.SystemSettingsId;
            }
            
        }
    }
}
